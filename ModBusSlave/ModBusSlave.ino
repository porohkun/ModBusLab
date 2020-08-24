/*
 Name:		ModBusSlave.ino
 Created:	8/24/2020 11:24:37 PM
 Author:	Poroh
*/

#include "Modbusino.h"
#include "Button.h"

#define MY_BUTTON 0x03

/* Initialize the slave with the ID 1 */
ModbusinoSlave _modbusinoSlave(1);

uint16_t _registers[10] = { 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

void setup()
{
	pinMode(LED_BUILTIN, OUTPUT);

	byte buttons[BTN_COUNT] = { MY_BUTTON, 0xff, 0xff, 0xff, 0xff };
	Button.SetButtons(buttons);

	_modbusinoSlave.setup(115200);
}


void loop()
{
	Button.Loop();
	digitalWrite(LED_BUILTIN, Button.GetState(MY_BUTTON) ? HIGH : LOW);

	if (Button.GetDown(MY_BUTTON))
		_registers[MY_BUTTON]++;

	_modbusinoSlave.loop(_registers, 10);
}
