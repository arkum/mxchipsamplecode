#include "HTS221Sensor.h"
#include "AZ3166WiFi.h"
#include "DevKitMQTTClient.h"
#include "parson.h"
#include "http_client.h"
#include "StringUtils.h"

DevI2C *i2c;
HTS221Sensor *sensor;
static bool hasWifi = false;
int messageCount = 1;

static void InitWifi()
{
  if (WiFi.begin() == WL_CONNECTED)
  {
    IPAddress ip = WiFi.localIP();
    Screen.print(0, ip.get_address());
    hasWifi = true;
    Screen.print(1, "Running... \r\n");
  }
  else
  {
    hasWifi = false;
    Screen.print(0, "No Wi-Fi\r\n ");
  }
}

static void MessageCallback(const char* payLoad, int size){
  Screen.print(1, payLoad, true);
}

void setup() {
  // put your setup code here, to run once:
  Screen.init();
  InitWifi();
  i2c = new DevI2C(D14, D15);
  sensor = new HTS221Sensor(*i2c);
  sensor->init(NULL);
  DevKitMQTTClient_Init(true);
  DevKitMQTTClient_SetMessageCallback(MessageCallback);
}

void loop() {
  // put your main code here, to run repeatedly:
  float temperature = 0;
  sensor->enable();
  sensor->getTemperature(&temperature);
  char buf[6];
  dtostrf(temperature,4,2,buf);
  Screen.print(2, buf);
  sensor->disable();
  sensor->reset();

  JSON_Value *root_value = json_value_init_object();
  JSON_Object *root_object = json_value_get_object(root_value);
  json_object_set_number(root_object, "Id", messageCount++);
  json_object_set_string(root_object, "temperature", buf);
  char* messagestring = json_serialize_to_string(root_value);
  EVENT_INSTANCE* message =  DevKitMQTTClient_Event_Generate(messagestring, MESSAGE);
  DevKitMQTTClient_SendEventInstance(message);

  String messagebody = String(messagestring);
  HTTPClient client = HTTPClient(HTTP_POST, "<functionurl>");
  client.set_header("Content-Type", "application/json");
  const Http_Response* response = client.send(messagebody.c_str(), messagebody.length());
  if(response != NULL){
    Screen.print(3, response->body);
  }

  json_free_serialized_string(messagestring);
  json_value_free(root_value);

  delay(2000);

}
