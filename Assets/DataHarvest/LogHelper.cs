// Provides convenience methods for logging
public static class LogHelper {
    public static void Log(string prelude, object caller, string str) {
        UnityEngine.Debug.Log("[ " + prelude + " ] " + caller.GetType().Name + ": " + str);
    }

    public static void Info(object caller, string str) {
        Log("INFO", caller, str);
    }
    
    public static void Warn(object caller, string str) {
        Log("WARNING", caller, str);
    }
    
    public static void Error(object caller, string str) {
        Log("ERROR", caller, str);
    }

    public static void Ok(object caller, string str) {
        Log("OK", caller, str);
    }

    public static void Debug(object caller, string str) {
        Log("DEBUG", caller, str);
    }
}
