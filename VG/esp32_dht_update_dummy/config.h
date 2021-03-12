#define WIFI_INTERVAL 1000
unsigned long currentMillis;
const char* ntpServer = "pool.ntp.org";
unsigned long epochTime; 

#define MESSAGE_LEN_MAX 60
#define SCHOOL "Nackademin IoT 20"
#define NAME "Julia Gu"
#define LOCATION "Gothenburg"
#define LATITUDE "57.7089"
#define LONGITUDE "11.9746"
#define SENSORTYPE "dht"

#define DHT_INTERVAL 5000
#define DHT_TYPE DHT11
#define DHT_PIN 21
unsigned long PREV_DHT_MILLIS = 0;
float humidity = 0;
float current_temperature = 0;

bool messagePending = false;
