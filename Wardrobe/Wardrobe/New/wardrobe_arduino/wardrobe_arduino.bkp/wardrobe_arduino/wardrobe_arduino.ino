/**  
 *  wardrobe_arduino.ino
 *  
 *  REGAME-VR Wardrobe Arduino
 *  Control software for wardrobe targets.
 *  
 *  @author Hamilton Kibbe <ham@hamiltonkib.be>
 *  
 *  Theory of operation
 *  -------------------
 *  
 *  
 *  
 *  Hardware Connections
 *  --------------------
 *         
 *  ----------+
 *  Mega 2560 |
 *        |
 *         26 | => HX711[Section 1] DAT
 *         27 | => HX711[Section 1] CLK
 *         28 | => HX711[Section 2] DAT
 *         29 | => HX711[Section 2] CLK
 *         30 | => HX711[Section 3] DAT
 *         31 | => HX711[Section 3] CLK
 *         32 | => HX711[Section 4] DAT
 *         33 | => HX711[Section 4] CLK
 *         34 | => HX711[Section 5] DAT
 *         35 | => HX711[Section 5] CLK
 *         36 | => HX711[Section 6] DAT
 *         37 | => HX711[Section 6] CLK
 *         38 | => HX711[Section 7] DAT
 *         39 | => HX711[Section 7] CLK
 *            |
 *         40 | => LED DATA IN
 *            |
 *         50 | => LEFT DOOR SENSOR+       
 *         51 | => RIGHT DOOR SENSOR+
 *         52 | => DRAWER SENSOR+
 *            |
 *  ----------+
 *     
 *         
 *      
 * External Dependencies
 * ---------------------
 * The software depends on the following third-party libraries:
 * 
 *   - Adafruit NeoPixel library
 *     https://github.com/adafruit/Adafruit_NeoPixel
 * 
 *   - HX711 Library
 *     https://github.com/bogde/HX711
 *   
 *   
 */


/* Project headers */
#include "door_sensor.h"
#include "leds.h"

/* Third party headers */
#include "HX711.h"

/* Standard libraries */
#include <stdint.h>


/* Constants */

/* Nunmber of targets connected to this device */
#define NUMBER_OF_TARGETS           (7)
#define NUMBER_OF_DOORS             (3)


/* Led Params */
#define LED_PIN                     (40)

/* Door sensor Params */
#define DOOR_SENSOR_0_PIN           (50)
#define DOOR_SENSOR_1_PIN           (51)
#define DOOR_SENSOR_2_PIN           (52)

/* Loadcell Params */
#define SCALE_0_DOUT_PIN              (26)
#define SCALE_0_CLK_PIN               (27)
#define SCALE_1_DOUT_PIN              (28)
#define SCALE_1_CLK_PIN               (29)
#define SCALE_2_DOUT_PIN              (30)
#define SCALE_2_CLK_PIN               (31)

#define SCALE_3_DOUT_PIN              (32)
#define SCALE_3_CLK_PIN               (33)
#define SCALE_4_DOUT_PIN              (34)
#define SCALE_4_CLK_PIN               (35)
#define SCALE_5_DOUT_PIN              (36)
#define SCALE_5_CLK_PIN               (37)
#define SCALE_6_DOUT_PIN              (38)
#define SCALE_6_CLK_PIN               (39)

#define SERIAL_BAUD_RATE            (19200)

#define UPDATE_PERIOD_MS            (200)




typedef struct TargetState {
  float   weight[NUMBER_OF_TARGETS];
  uint8_t doorState[NUMBER_OF_DOORS];
} TargetState_t;

typedef struct CommandState {
  uint8_t tare;
  LedColor_t led_colors[NUMBER_OF_TARGETS];
  uint8_t scale_id;
} CommandState_t;


TargetState_t   target_state = {0};
CommandState_t  command_state = {0};
static char     rx_buffer[128] = {0};
static long     last_tx_time = 0;


DoorSensor doorSensors[NUMBER_OF_DOORS] = {
  DoorSensor(DOOR_SENSOR_0_PIN),
  DoorSensor(DOOR_SENSOR_1_PIN),
  DoorSensor(DOOR_SENSOR_2_PIN)
};


HX711 scales[NUMBER_OF_TARGETS] = {
  HX711(SCALE_0_DOUT_PIN, SCALE_0_CLK_PIN),
  HX711(SCALE_1_DOUT_PIN, SCALE_1_CLK_PIN),
  HX711(SCALE_2_DOUT_PIN, SCALE_2_CLK_PIN),
  HX711(SCALE_3_DOUT_PIN, SCALE_3_CLK_PIN),
  HX711(SCALE_4_DOUT_PIN, SCALE_4_CLK_PIN),
  HX711(SCALE_5_DOUT_PIN, SCALE_5_CLK_PIN),
  HX711(SCALE_6_DOUT_PIN, SCALE_6_CLK_PIN)
};

LedController ledController(NUMBER_OF_TARGETS, LED_PIN);




/* Arduino Setup **********************************************************/
void setup() {

  /* Set up com port */
  Serial.begin(SERIAL_BAUD_RATE);
  Serial.setTimeout(5);


  /* Set up led Controller */
  ledController.initialize();
}



/* Arduino Main Loop ******************************************************/
void loop() {
  long current_time;

  /* Update the sensor state */
  uint8_t target_index;
  uint8_t door_index;
  
  for (door_index = 0; door_index < NUMBER_OF_DOORS; ++door_index)
  {
    target_state.doorState[door_index] = doorSensors[door_index].getDoorState();
  }
  
  
  for (target_index = 0; target_index < NUMBER_OF_TARGETS; ++target_index)
  {

    /* Reset scales */
    target_state.weight[target_index] = 0;
  }

  /* Update LEDs */
   ledController.setColors(command_state.led_colors);

  /* Reading from scales is really slow. Only read from the currently enabled one.*/
  if (command_state.scale_id > 0)
  {
    target_state.weight[command_state.scale_id - 1] = scales[command_state.scale_id - 1].get_value();
  }

  /* If theres an incoming command message, parse it */
  if (Serial.available() >= 0)
  {

    int idx;
    int bytes_read = Serial.readBytesUntil('\n', rx_buffer, 128);
    for (idx = 0; idx < NUMBER_OF_TARGETS; ++idx)
     {
        command_state.led_colors[idx] = (LedColor_t)rx_buffer[idx];
     }

     command_state.scale_id = rx_buffer[idx++];

     /* Parse the tare byte out */
     command_state.tare = rx_buffer[idx];
     if (command_state.tare)
     {
        for (idx = 0; idx < NUMBER_OF_TARGETS; ++idx)
        {
          scales[idx].tare();
        }
     }
  }


  
  /* If it's time to send an update send ot matlab */
  current_time = millis();
  if ((current_time - last_tx_time) > UPDATE_PERIOD_MS)
  {
    for (int target = 0; target < NUMBER_OF_TARGETS; ++target)
    {
      Serial.print("w");
      Serial.print(target + 1);
      Serial.print(":");
      Serial.print(target_state.weight[target]);
      Serial.print(",");
    }

    for (int door_id = 0; door_id < NUMBER_OF_DOORS; ++door_id)
    {
      Serial.print("d");
      Serial.print(door_id + 1);
      Serial.print(":");
      Serial.print(target_state.doorState[door_id]);
      Serial.print(",");
    }
    Serial.print('\n');

    last_tx_time = current_time;
  }
}
