#!/bin/bash

echo "Updating the system and making sure mono is installed"
sudo apt-get update
sudo apt-get -yq install mono-runtime
echo "Installation started for Nabaztag.Server"
echo "Stopping any previous service"
sudo systemctl is-active --quiet nabd.socket && sudo systemctl stop nabd.socket
sudo systemctl disable nabd.socket
sudo systemctl is-active --quiet nabd && sudo systemctl stop nabd
sudo systemctl disable nabd
sudo systemctl is-active --quiet nabaztag-server && sudo systemctl stop nabaztag-server
echo "Preparing service and config files"
dir=`pwd`
IFS='/' read -a subdir <<< ${dir}
search='s+/home/pi/nabsrv+'${dir}'+g'
sudo sed -i ${search} nabaztag-server.service
echo "Installing service in systemd"
sudo cp nabaztag-server.service /etc/systemd/system/nabaztag-server.service
sudo systemctl daemon-reload
sudo systemctl start nabaztag-server
sudo systemctl enable nabaztag-server
echo "Service installed and sarted"
echo "Rebooting"
sudo reboot
