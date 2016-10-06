package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"

	"github.com/gorilla/mux"
)

type dataColor struct {
	R float64
	G float64
	B float64
}

type userData struct {
	Devicedata deviceData
	Gamedata   []gameData
}

type deviceData struct {
	ScreenDPI    float64
	ScreenHeight int
	ScreenWidth  int
}

type gameData struct {
	AnswerCorrect         bool
	ResponseTime          float64
	TimeSinceStart        float64
	BladeQuDistance       float64
	BladeQuBorderDistance float64
	CorrectColor          dataColor
	WrongColors           []dataColor
	BackgroundColor       dataColor
	BorderRadius          float64
	NumberOfBlades        uint
}

type icarusData struct {
	Appdata struct {
		Uuid       string
		Voiceover  bool
		Device     string
		Lang       string
		Appname    string
		Appversion int32
	}
	Debug     bool
	Userdata  userData
	Timestamp struct {
		Utc  string
		User string
	}
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
