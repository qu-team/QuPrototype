package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"

	"github.com/gorilla/mux"
)

type dataColor struct {
	R float64 `json:"r"`
	G float64 `json:"g"`
	B float64 `json:"b"`
}

type userData struct {
	Devicedata deviceData `json:"devicedata"`
	Gamedata   []gameData `json:"gamedata"`
}

type deviceData struct {
	ScreenDPI    float64 `json:"screenDPI"`
	ScreenHeight int     `json:"screenHeight"`
	ScreenWidth  int     `json:"screenWidth"`
}

type gameData struct {
	LevelNum              int         `json:"levelNum"`
	AnswerCorrect         bool        `json:"answerCorrect"`
	ResponseTime          float64     `json:"responseTime"`
	TimeSinceStart        float64     `json:"timeSinceStart"`
	BladeQuDistance       float64     `json:"bladeQuDistance"`
	BladeQuBorderDistance float64     `json:"bladeQuBorderDistance"`
	Colors                []dataColor `json:"colors"`
	GuessedColor          int         `json:"guessedColor"`
	CorrectColor          int         `json:"correctColor"`
	BackgroundColor       dataColor   `json:"backgroundColor"`
	BorderRadius          float64     `json:"borderRadius"`
	NumberOfBlades        int         `json:"numberOfBlades"`
}

type icarusData struct {
	Appdata struct {
		Uuid       string `json:"uuid"`
		Voiceover  bool   `json:"voiceover"`
		Device     string `json:"device"`
		Lang       string `json:"lang"`
		Appname    string `json:"appname"`
		Appversion int32  `json:"appversion"`
	} `json:"appdata"`
	Debug     bool     `json:"debug"`
	Userdata  userData `json:"userdata"`
	Timestamp struct {
		Utc  string `json:"utc"`
		User string `json:"user"`
	} `json:"timestamp"`
}

func postHandler(rw http.ResponseWriter, req *http.Request) {
	decoder := json.NewDecoder(req.Body)
	var data icarusData
	ok := "OK"
	if err := decoder.Decode(&data); err != nil {
		log.Println("Format is not correct!")
		ok = "KO"
	} else {
		log.Println("Format is OK")
	}
	if b, err := json.MarshalIndent(data, "", "    "); err != nil {
		fmt.Printf("error: %s\n", err.Error())
	} else {
		fmt.Println(string(b))
	}
	fmt.Printf("--------------EOF (%s, %d bytes)\n\n", ok, req.ContentLength)
}

func main() {
	router := mux.NewRouter()
	POST := router.Methods("POST").Subrouter()

	POST.HandleFunc("/", postHandler)

	http.Handle("/", router)
	log.Println("Listening on 0.0.0.0:8000")
	http.ListenAndServe(":8000", nil)
}
