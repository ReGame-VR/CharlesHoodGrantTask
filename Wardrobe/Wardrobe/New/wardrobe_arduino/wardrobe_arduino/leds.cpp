
#include "leds.h"


LedController::LedController(uint8_t number_of_sections, uint8_t led_pin)
{
  this->num_sections = number_of_sections;
  this->pixel = Adafruit_NeoPixel(number_of_sections * LEDS_PER_PIXEL, led_pin, NEO_GRB + NEO_KHZ800);

  /* Allocate buffer for color values */
  //size_t buffer_size = number_of_sections * sizeof(uint32_t);
  //this->color_values = (uint32_t*)malloc(buffer_size);
  
  /* Set current color state to some invalid color so the first write updates the pixel */
  //memset(this->color_values, 0, buffer_size);
}


void LedController::initialize() {
  
  /* Initialize NeoPixel class instance */
  this->pixel.begin();

  /* Set leds to default state */
  this->clear();

  this->rainbow();
}

void LedController::setColors(LedColor_t* colors)
{
  /* color value as used by library */

  bool changed = false;
  uint32_t color_value;
  uint8_t section;
  uint16_t pixel_index;
  uint16_t subpixel_index;
  
  for (section = 0; section < this->num_sections; ++section)
  {
    switch (colors[section])
    {
      case LED_COLOR_OFF:
        color_value = this->pixel.Color(0, 0, 0);
        break;
      case LED_COLOR_GREEN:
        color_value = this->pixel.Color(0, LED_BRIGHTNESS, 0);
        break;
      case LED_COLOR_YELLOW:
        color_value = this->pixel.Color(LED_BRIGHTNESS, LED_BRIGHTNESS, 0);
        break;
      case LED_COLOR_RED:
        color_value = this->pixel.Color(LED_BRIGHTNESS, 0, 0);
        break;
      case LED_COLOR_BLUE:
        color_value = this->pixel.Color(0, 0, LED_BRIGHTNESS);
        break;
      case LED_COLOR_WHITE:
        color_value = this->pixel.Color(LED_BRIGHTNESS, LED_BRIGHTNESS, LED_BRIGHTNESS);
        break;
      default:
        color_value = this->pixel.Color(0, 0, 0);
        break;
    }


    /* Set the color for each LED on the pixel board */
    for (subpixel_index = 0; subpixel_index < LEDS_PER_PIXEL; ++subpixel_index)
    {
      pixel_index = (section * LEDS_PER_PIXEL) + subpixel_index;
      this->pixel.setPixelColor(pixel_index, color_value);
    }

    /*
    if (color_value != this->color_values[section])
    {
      changed = true;
    }
    this->color_values[section] = color_value;
    */
    this->pixel.show();
  }
  /*
  if (changed)
  {
    this->pixel.show();
  }
  */
}


void LedController::clear()
{
  /* Clear the current color state array */
  //memset(this->color_values, 0, this->num_sections * sizeof(uint32_t));

  /* Reset NeoPixel library's framebuffer and write it out to the LEDS */
  this->pixel.clear();
  this->pixel.show();
}


void LedController::rainbow() {
  uint16_t i, j;

  for(j=0; j<256; j++) {
    for(i=0; i<this->pixel.numPixels(); i++) {
      this->pixel.setPixelColor(i, this->wheel((i+j) & 255));
    }
    this->pixel.show();
    delay(20);
  }
}


uint32_t LedController::wheel(uint8_t WheelPos) {
  WheelPos = 255 - WheelPos;
  if(WheelPos < 85) {
    return this->pixel.Color(255 - WheelPos * 3, 0, WheelPos * 3);
  }
  if(WheelPos < 170) {
    WheelPos -= 85;
    return this->pixel.Color(0, WheelPos * 3, 255 - WheelPos * 3);
  }
  WheelPos -= 170;
  return this->pixel.Color(WheelPos * 3, 255 - WheelPos * 3, 0);
}

