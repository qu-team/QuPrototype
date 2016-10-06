# We use JDK 1.6 for compatibility 
javac -source 1.6 -target 1.6 qu/GetLocale.java && rm -rf getlocale.jar && jar cf getlocale.jar qu/GetLocale.class
