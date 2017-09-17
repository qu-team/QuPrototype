using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System.IO;
using System;
#if UNITY_WSA
    using System.Collections.Generic;
    using System.Linq;
#endif

public sealed class CrashHandler {

    readonly string path;
    // Used to spawn coroutines
    readonly MonoBehaviour mb;
    LocalDataHandler localData;

    float latestSavedExceptionTime;
    string latestLogString;
    string latestStackTrace;
    LogType latestType;

    public CrashHandler(MonoBehaviour mb) {
        path = Application.persistentDataPath;
        this.mb = mb;
        localData = new LocalDataHandler(path);
        LogHelper.Info(this, "request url: " + GameManager.Instance.AppConfig.GetRequestURL());
    }

    public void SendLocalData() {
        mb.StartCoroutine(SendLocal());
    }

    // Global error handler
    public void OnApplicationError(string logString, string stackTrace, LogType type) {
        // Ignore minor errors
        if (type == LogType.Log || type == LogType.Warning) return;

        // Prevent logging many times the same error
        if (type == latestType && logString == latestLogString && stackTrace == latestStackTrace
                && (Time.time - latestSavedExceptionTime) < 60)
        {
            return;
        }

        var now = DateTime.Now;
        var data = Protocol.WrapUserData(new IcCrash {
            exception = new IcCrashLogData {
                type = type.ToString(),
                logString = logString,
                stackTrace = stackTrace
            }
        });
        mb.StartCoroutine(SendCrashLog(data));

        latestSavedExceptionTime = Time.time;
        latestType = type;
        latestStackTrace = stackTrace;
        latestLogString = logString;
    }

    IEnumerator SendCrashLog(string data, string sourceFile = null) {
        var request = new UnityWebRequest(GameManager.Instance.AppConfig.GetRequestURL());
        request.method = "POST";

        byte[] payload = Encoding.UTF8.GetBytes(data);
        var uploader = new UploadHandlerRaw(payload);
        request.uploadHandler = uploader;
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

#if UNITY_5
        if (!request.isError) {
#else
        if (!request.isNetworkError) {
            LogHelper.Ok(this, "Crash report sent successfully");
#endif
            if (sourceFile != null)
                File.Delete(sourceFile);
        } else {
            LogHelper.Warn(this, "error: " + request.error);
            LogHelper.Info(this, "saving crash report locally");
            // Only save locally if this data doesn't already come from a local file
            if (sourceFile == null)
                localData.SaveCompressed(data, GenerateFileName());
        }

        yield return null;
    }

    string GenerateFileName() {
        var now = DateTime.Now;
        return string.Format("crashreport_{0}-{1}-{2}T{3}:{4}:{5}_{6}",
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }

    public IEnumerator SendLocal() {
        LogHelper.Info(this, "checking for local data...");

#if UNITY_WSA
        string[] files = new List<string>(Directory.GetFiles(path)).Where(file => {
                return file.StartsWith("crashreport_") && file.EndsWith(".gz");
        }).ToArray();
#else
        string[] files = Directory.GetFiles(path, "crashreport_*.gz");
#endif
        foreach (string fname in files) {
            LogHelper.Debug(this, "loading file " + fname);
            string datastr = localData.LoadCompressed(fname);
            if (datastr == null) {
                LogHelper.Info(this, "data in file " + fname + " is empty: deleting.");
                File.Delete(fname);
                continue;
            }

            yield return SendCrashLog(datastr, fname);
        }

        yield return null;
    }
}
