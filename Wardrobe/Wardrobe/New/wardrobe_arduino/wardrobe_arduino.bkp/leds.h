
#ifndef LEDS_H_
#define LEDS_H_

#include <Adafruit_NeoPixel.h>
#include <stdint.h>

/* LED Brightness 0-255 */
#define LED_BRIGHTNESS  (100)
#define LEDS_PER_PIXEL  (7)


typedef enum LedColor {
  LED_COLOR_OFF,
  LED_COLOR_GREEN,
  LED_COLOR_YELLOW,
  LED_COLOR_RED,
  LED_COLOR_BLUE,
  LED_COLOR_WHITE,
} LedColor_t;


class LedController {
  public:
    LedController(uint8_t number_of_pixels, uint8_t led_pin);
    ~LedController() {};
    void initialize();
    void setColors(LedColor_t* colors);
    void turnOff();
  private:
    uint8_t num_sections;
    uint32_t color_values[7];
    Adafruit_NeoPixel pixel;
    void writeColorToLeds();
};

#endif
