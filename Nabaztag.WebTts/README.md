# Nabaztag Text to Speech Application

This Text to Speech application works perfectly on the Nabaztag and perfectly integrated with pynab. This allows a very smooth usage of it.

# Installing the service

You can make things easy by using the release and the installation process part of it:
- Connect in ssh to your Nabaztag
- Create a directory in your /home/pi for example `webtts`. If the pynab service is not installed in his default directory, you'll need to create a directory at the same root level.
- Download the latest Nabaztag.WebTts release and copy it into `webtts`
- ```cd webtts```
- ```sudo bash installtts.sh```
Be patient, it can take quite some time as it install mono complete. 
- Then open a browser on your PC or Phone: http://yournabaztagipaddress:8888, it should look like that:

![tts](/docs/tts.jpg)

# Setting the Text to Speech service

You'll need to create an Azure account and then setup an Azure cognitive Service. No stress, this is free to create, also for a normal Text to Speech usage, there is a free service that should be largely enough for you!

- Go to https://azure.microsoft.com/ and create an account if you don't have one. A credit card may be required, but it won't be used if you select the free Text to Speech service
- Once created, connect to the Azure Port: https://portal.azure.com
- Then click on `Create a Resource`

![create](/docs/create.jpg)

- Then seach for `Speech` and click `Create`

![speech](/docs/speech.jpg)

- Then fill the form like this:

![details](/docs/details.jpg)

If you don't want to pay, select the free service F0. If you really do an intense usage, the prices are extremly cheap anyway, it will cost you few cents of Euro/Dolar per month! Also, make sure you'll select a region close by where you are located.

- Click on `Create` and patient a bit, the resource is getting created.
- Go in the resource and click `Keys and Endpoint`

![keys](/docs/keys.jpg)

**Importannt**: you'll need to copy the key and the Endpoint

- Go to the Nabatztag URL and click on Settings:

![settings](/docs/settings.jpg)

**Important**: paste the key and the Endpoint and click `Save`. If all goes right, you'll now be able to select your voice:

![selectvoice](/docs/selectvoice.jpg)

**Important**: so far the Nabaztag.WebTts supports only the language you'll select. So Speech to Text will only be in the language you'll select in this drop down list. For better result, you can choose a `Neural` speaker, they seems much more natural.

*Notes*: 
- Other settings should be just fine.
- So far UI is supported in French and English. Anything else will fall down into English. If you're interested in getting your own language, please open an issue or a PR with the [localized resource file](./Resources/Tts.resx)

- Test it! write something and just click the say it button!

![working](/docs/working.jpg)

# Using the REST API

A simple REST API is available to use programatically. You can use the service in GET or POST: /api/Speak

So it's as simple as: http://nabaztagipaddress:8888/api/Speak/dis%20quelque%20chose

it will return `ok`if success and a detailed error message in case of failure.