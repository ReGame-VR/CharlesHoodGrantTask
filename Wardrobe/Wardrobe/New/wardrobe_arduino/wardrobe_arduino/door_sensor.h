
#ifndef DOOR_SENSOR_H_
#define DOOR_SENSOR_H_

#include <Arduino.h>
#include <stdint.h>

typedef enum DoorState {
  DOOR_CLOSED = 0,
  DOOR_OPEN
}DoorState_t;


class DoorSensor
{
  public:
    DoorSensor(uint8_t pin) : pin(pin) {};
    ~DoorSensor() {};
    void initialize() {pinMode(pin, INPUT_PULLUP);};
    DoorState_t getDoorState() {
      pinMode(this->pin, INPUT_PULLUP); 
      return (DoorState_t)digitalRead(this->pin);
    };

  private:
    uint8_t pin;   
};


#endif
