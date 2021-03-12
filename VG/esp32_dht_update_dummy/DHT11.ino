void sendDhtMessage() {
  current_temperature = random(400,700)/100.0;
  humidity = random(20,40);
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
