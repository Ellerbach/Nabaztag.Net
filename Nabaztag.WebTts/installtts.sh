#!/bin/bash

echo "Updating the system and making sure mono is installed"
sudo apt-get update
sudo apt-get -yq install mono-complete
echo "Installation started for Nabaztag.WebTts"
echo "Stopping any previous service"
sudo systemctl is-active --quiet nabaztag-tts && sudo systemctl stop nabaztag-tts
echo "Preparing service and config files"
dir=`pwd`
IFS='/' read -a subdir <<< ${dir}
search='s+/home/pi/webtts+'${dir}'+g'
sudo sed -i ${search} nabaztag-tts.service
searchdir='s+"tts"+"'${subdir[-1]}'"+g'
sudo sed -i ${searchdir} ttsconfig.json
echo "Installing service in systemd"
sudo cp nabaztag-tts.service /etc/systemd/system/nabaztag-tts.service
sudo systemctl daemon-reload
sudo systemctl start nabaztag-tts
echo "Service installed and sarted"
ipadd=$(ifconfig | sed -En 's/127.0.0.1//;s/.*inet (addr:)?(([0-9]*\.){3}[0-9]*).*/\2/p')
echo "Connect from your PC or Phone to http://"${ipadd}":8888 and be patient, the service take up to one minute to execute the first time"
