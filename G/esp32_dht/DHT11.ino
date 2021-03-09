void initDHT() {
  pinMode(DHT_PIN, INPUT);
  dht.begin();
}

void sendDhtMessage() {
  current_temperature = dht.readTemperature();
  humidity = dht.readHumidity();
  epochTime = getTime();
  
  if ((currentMillis - PREV_DHT_MILLIS) >= DHT_INTERVAL 
        && !messagePending 
        && !std::isnan(current_temperature) && !std::isnan(humidity) 
        && epochTime > 28800) 
  {
    PREV_DHT_MILLIS = currentMillis;
    messagePending = true;
    
    char payload[MESSAGE_LEN_MAX];    
    DynamicJsonDocument doc(sizeof(payload));
    
    doc["temperature"] = current_temperature;
    doc["humidity"] = humidity;   
    doc["ts"] = epochTime;
    serializeJson(doc, payload);
    
    sentAzureIothub(payload);
  }
}
