
//Carrega a biblioteca LiquidCrystal
#include <LiquidCrystal.h>
 
//Define os pinos que serão utilizados para ligação ao display
LiquidCrystal lcd(12, 11, 5, 4, 3, 2);
 byte grau[8] = {
    0b00000,
    0b01110,
    0b10001,
    0b10001,
    0b01110,
    0b00000,
    0b00000,
    0b00000
  };
void setup()
{
  lcd.begin(16, 2);
  Serial.begin(9600);
  
  
  // Carregar o caractere customizado para a posição 0
  lcd.createChar(0, grau);
}
 
void loop()
{
  if(Serial.available()){
    int temperature = Serial.parseInt();
    if(temperature == 0){
      return;
    }
    lcd.clear();
    lcd.setCursor(4, 0);
    lcd.print("CPU Temp");
    lcd.setCursor(5, 1);
    lcd.print(temperature);
    lcd.print(" ");
    lcd.write(byte(0));  // Exibe o símbolo de grau
    lcd.print("C");
  }
   
}