# Ar-IOT
This Project is an experiment of combining AugmentedReality with IoT, and how to involve several components in such a solution. This projects use Win Iot Core as server side on a Raspberry Pi, and SWIFT for client app on a IPhone, and C for Arduino temperature readings.

Arduino is wired up directly to the Raspberry PI, on RX, TX. 

The project contains a server for reading and distributing temperature reading from a sensor.
The folder SensorServer contains early version of servers which is designed for running on Win IoT Core.

RaspberryServer is a server reading values from an Arduino.

EasyServer is a simple web server for serving values from the RaspberryServer

For the Arduino temperature reading use the file: termometer.ino

Additionaly there is an early prototype for an swift app added here, but only the mainclasses are published at this point.
The code in this project is by no means finished, it is an ongoing project and is a bit buggy.


there are some files kept for debugging purpose
Mainpage.xaml app
