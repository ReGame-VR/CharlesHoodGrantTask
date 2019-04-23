
#include "leds.h"


LedController::LedController(uint8_t number_of_sections, uint8_t led_pin)
{
  this->num_sections = number_of_sections;
  this->pixel = Adafruit_NeoPixel(number_of_sections * LEDS_PER_PIXEL, led_pin, NEO_GRB + NEO_KHZ800);

  /* Set current color state to some invalid color so the first write updates the pixel */
  for (uint8_t section = 0; section < this->num_sections; ++section)
  {
    this->color_values[section] = 0xffffffff;
  }
}


void LedController::initialize() {
  this->pixel.begin();
  this->turnOff();  
}

void LedController::setColors(LedColor_t* colors)
{
  /* color value as used by library */

  bool changed = false;
  uint32_t color_value;
  
  for (uint8_t idx = 0; idx < this->num_sections; ++idx)
  {
    switch (colors[idx])
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

    if (color_value != this->color_values[idx])
    {
      changed = true;
    }
    this->color_values[idx] = color_value;
  }

  if (changed)
  {
    this-> writeColorToLeds();
  }
}


void LedController::turnOff()
{
  LedColor_t new_colors[this->num_sections];
  for (uint8_t idx = 0; idx < this->num_sections; ++idx)
  { 
    new_colors[idx] = LED_COLOR_OFF;
  }
  this->setColors(new_colors);
}


void LedController::writeColorToLeds()
{
    uint8_t pixel_index = 0;
    
    /* Write color to each pixel, then update the string */
    for (int8_t section = 0; section < this->num_sections; ++section)
    {
      uint32_t color_value = this->color_values[section];
      for (uint8_t pxl_id = 0; pxl_id < LEDS_PER_PIXEL; ++pxl_id)
      {
        pixel_index = (section * LEDS_PER_PIXEL) + pxl_id;
        this->pixel.setPixelColor(pixel_index, color_value);
      }
    }
    this->pixel.show();
}



