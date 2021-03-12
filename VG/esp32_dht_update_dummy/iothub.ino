void initIotHub(){
  Esp32MQTTClient_Init((uint8_t *)connectionString, true);
  Esp32MQTTClient_SetSendConfirmationCallback(SendConfirmationCallback);
}

void SendConfirmationCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result){
  if(result == IOTHUB_CLIENT_CONFIRMATION_OK){
    Serial.println("Confirmed");
    messagePending = false;
  }
}

void sentAzureIothub(char* payload){
  EVENT_INSTANCE *message = Esp32MQTTClient_Event_Generate(payload, MESSAGE);
  Esp32MQTTClient_Event_AddProp(message, "sensorType", SENSORTYPE);
  Esp32MQTTClient_Event_AddProp(message, "school", SCHOOL);
  Esp32MQTTClient_Event_AddProp(message, "name", NAME);
  Esp32MQTTClient_Event_AddProp(message, "role", "student");
  Esp32MQTTClient_Event_AddProp(message, "location", LOCATION);
  Esp32MQTTClient_Event_AddProp(message, "latitude", LATITUDE);
  Esp32MQTTClient_Event_AddProp(message, "longitude", LONGITUDE);
  Esp32MQTTClient_SendEventInstance(message);
}
