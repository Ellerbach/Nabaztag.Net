# Nabaztag.Net to have a full interaction with the pynab service

The aim of this project is to fully interact with the [pynab service](https://github.com/nabaztag2018/pynab).

I love .NET Core, tons of people do as well. Tis project is about being able to run a .NET Core application to interact with the new Nabaztag and the services offered. This library is still under development and may contains bugs or need improvments. Feel free to open issues and to PR!

## Running the .NET Core application on Nabaztag

.NET Core is not supported directly on a Raspeberry Pi zero as the architecture is ARM v6 while .NET Core supports ARM architectures stating at ARM v7. Work is being done to port it but in the mean time, you can use Mono to run your code. Here are the steps to install Mono on your Raspberry Pi zero.

Connect in SSH to the RPI and install mono:

```bash
sudo apt-get update
sudo apt-get install mono-complete
```

Then a lot of patience during the instalation. This may take a while, really a lot of time. The Nabaztag will sometimes even stop lighting and interacting.

Then you need to normally build your application on your development machine:

```cmd
dotnet build
```

Then you'll need to copy the \bin\debug\netcoreapp3.1 directory into the Nabaztag. You can use the WinSCP for example and it will look like that:
![screen capture](/docs/wincsp.jpg)

On the RPI, go into the created directory, in my case, I called it mono and run the application:

```bash
 mono Nabaztag.Net.Sample.dll localhost
```

And that's it! Enjoy playing with your Nabaztag using .NET Core :-)

## Running the .NET Core application on a remote machine

As you can't deploy .NET Core application on the Raspeberry Pi zero, you need to deploy it somewhere on your network and make sure your rabbit will listen to request coming from outside of it's body :-) See [here how setup the connection](https://github.com/nabaztag2018/pynab/issues/101)

To make it short, you need to open the socket to the outisde:

```bash
sudo sed -i -e 's|127.0.0.1|0.0.0.0|g' /lib/systemd/system/nabd.socket
sudo systemctl daemon-reload
sudo systemctl stop nabd.service
sudo systemctl restart nabd.socket
sudo systemctl start nabd.service
```

Then simply run the app from Visual Studio or a command prompt. You can ajust the default IP address in the code. In the sample app, it will try to connect to 192.168.1.145.

## Text To Speech (TTS) application

A specific Text to Speech application is available. It can run and be called from any place thru a REST API as well as from a web page. the application can be setup for the Azure Cognitive Services key.

All details [here](./Nabaztag.WebTts/README.md).

## Sample application

The sample application contains some examples, by running them, you'll get something like:

```
 mono Nabaztag.Net.Sample.dll localhost
Hello Nabaztag!
Nabaztag, please sleep :-)
Nabaztag status changed, new status is Idle
Your Nabaztag is sleeping
Waiting 10000 milliseconds
Nabaztag status changed, new status is Asleep
Nabaztag status changed, new status is Idle
Nabaztag, please wake up!
Your Nabaztag is awake
Test ears
Test leds
Setting interactive mode and all events, press a key to change mode
Nabaztag status changed, new status is Interactive
Your Nabaztag is in interactive mode and will receive all events
New button event: Down, Time: 1588086422.8434243
New button event: Up, Time: 1588086423.072463
New button event: Click, Time: 1588086423.2368298
New button event: Down, Time: 1588086423.6100624
New button event: Up, Time: 1588086423.7988737
New button event: Click, Time: 1588086423.9519281
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:
New ear event. Left:  Right:  Ear: Right, Time:

Reset all events
Nabaztag status changed, new status is Idle
Your Nabaztag is in idle mode
Your Nabaztag is in Idle mode and will receive all events
New button event: Down, Time: 1588086435.3736913
New button event: Up, Time: 1588086435.5484438
New button event: Click, Time: 1588086435.7085383
New button event: Down, Time: 1588086435.9400423
New button event: Up, Time: 1588086436.120674
New button event: Click, Time: 1588086436.2737155
New button event: Down, Time: 1588086439.523072
New button event: Up, Time: 1588086439.7745528
New button event: Click, Time: 1588086439.9398072
New ear event. Left: 0 Right: 10 Ear: , Time:

 This will send information to play when idel on the nabaztag as an info object
Waiting 10000 milliseconds
Nabaztag status changed, new status is Playing
Your Nabaztag is doing a respiration and a small choreography
Waiting 10000 milliseconds
Nabaztag status changed, new status is Idle
Playing meteo: Today strom, 25 degrees
Nabaztag status changed, new status is Playing
List played properly
Trying to the same meteo but with an exprired resquest. Nothing should be played
Nabaztag status changed, new status is Idle
As planned, this request has expired
Reset all events
Your Nabaztag is in idle mode

```

Notes: the version 0.7.4 does not send any events in interactive mode. It is recommended to use a more recent version. If you always want to get latest version, prefer the master branch.

## Nabaztag core class

The Nabaztag class exposes events, functions mapped on the Nabaztag communication protocol, a full Choreography builder and strongly typed classes for the communication protocol.

### Events

You can register to events easilly, once you have a create Nbaztag class, you can subscribe like this:

```csharp
_nabaztag.StateEvent += Nabaztag_StateEvent;
_nabaztag.ButtonEvent += Nabaztag_ButtonEvent;
_nabaztag.EarsEvent += Nabaztag_EarsEvent;
_nabaztag.AsrEvent += Nabaztag_AsrEvent;
```

You can consume them in a very straight forward way:

```csharp
private static void Nabaztag_ButtonEvent(object sender, ButtonEvent buttonEvent)
{
    Console.WriteLine($"New button event: {buttonEvent.Event}, Time: {buttonEvent.Time}");
}
```

Note that every event has a different event argument but principe remains the same.

You need as well to make sure you'll subscribe to events. You have 2 different ways: interactive and idle. Be aware that in the interactive mode, you'll block the execution of the other apps:

```chsarp
 var resp = _nabaztag.EventMode(ModeType.Interactive, new EventType[] { EventType.Button, EventType.Ears });
```

This will subscribe to asr, buttons and ears events:

```csharp
var resp = _nabaztag.EventMode(ModeType.Idle, new EventType[] { EventType.Button, EventType.Ears, EventType.Asr });
```

See the output of the sample application to undersant the kind of information you can receive.

### Protocole functions

All the functions mapping the communication protocol can be called with a specific GuiId and wiating for the answer or without. A mechanism of cancellation has been put in place. Waiting for a confirmation when another app is fully blocking may not be recommended.

For example, this will send an Info request, ask for confirmation of reception with a 30 second timeout:

```csharp
_nabaztag.Info(info, true, 30);
```

This will send a Command request without waiting for a confirmation:

```csharp
var resp = _nabaztag.Command(seq, false)
```

Byt default, if you'll just calling any of those functions, it will wait for the confirmation without any timeout. The following function will reset all events, wait for confirmation ad vitam eternam:

```csharp
var resp = _nabaztag.EventMode(ModeType.Idle, new EventType[] { });
```

## Audio files and path

By default, any file you'll give will be searched in any of the "sound" directory in the pynab main directory. The way the python code is looking at the file is using the core sound directory and look in it. So you can easilly hack it and place the sound you want to play in any directory on the SD Card. You'll just have to pass the directory in a relative path. For example "../../../../nabaztag/test.mp3" will play the file test.mp3 with is located in ~/nabaztag directory so the same home directory as the ~/pynab directory. So you can generate your own files for let's say Text to Speech for example, store them locally and ask using the regular interaction to play it.

Example, this will play a signature and a test message (my lovely French speaking accent):

```chsarp
var signature = new Sequence() { AudioList = new string[] { "../../../../nabaztag/sploc.mp3" } };
var body = new Sequence[] { new Sequence() { AudioList = new string[] { "../../../../nabaztag/test.mp3" } } };
var resp = _nabaztag.Message(signature, body, DateTime.MinValue);
if (resp.Status == Status.Ok)
    Console.WriteLine("List played properly");
else
    Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");
```

Note that the mp3 files has to be in the Raspberry Pi to work. The code can execute remotely but you must have the mp3 files locally.

## Choreography elements

Choreography are using elements de determine how you want to light the led, how long, moving the ears, paying random midi. In this version, you can read or generate your own Choreography files as well as generating some on the fligh.

You will find as well a [tool that dump any Choreography file](./Util/README.md) into a readable way. All the provided Choreography files are dumped for you to understand.

Key principles of a Choreography file:
- It all starts with a timeframe, in milliseconds. This is the unit that will be used later for all synchronization
- Switching one of the 5 led to a specific color ort a specific palette of colors (17 in total) or switching them off
- Moving the ears to a specific position in absolute or relative way
- ```Ifne``` which define a block of Choreography that are  used in the Tai-Chi

To create a Choreography, it's quite straight forward:

```csharp
Choreography choreography = new Choreography();
// delay: 0, OpCode: FrameDuration frameDuration: 16
choreography.SetFrameDuration(16);
// delay: 0, OpCode: SetLedPalette Led: Bottom, palette: 0
choreography.SetLedPalette(Led.Bottom, 0);
// delay: 0, OpCode: SetLedPalette Led: Left, palette: 1
choreography.SetLedPalette(Led.Left, 1);
// delay: 0, OpCode: SetLedPalette Led: Right, palette: 1
choreography.SetLedPalette(Led.Right, 1);
// delay: 0, OpCode: SetLedPalette Led: Nose, palette: 2
choreography.SetLedPalette(Led.Nose, 2);
// delay: 1, OpCode: SetLedoff Led: Left
choreography.SetLedOff(Led.Left, 1);
// delay: 0, OpCode: SetLedoff Led: Right
choreography.SetLedOff(Led.Right);
// delay: 0, OpCode: SetLedPalette Led: Center, palette: 1
choreography.SetLedPalette(Led.Center, 1);
// delay: 1, OpCode: SetLedoff Led: Center
choreography.SetLedOff(Led.Center, 1);
// delay: 1, OpCode: SetLedoff Led: Nose
choreography.SetLedOff(Led.Nose, 1);
// delay: 1, OpCode: SetLedPalette Led: Left, palette: 1
choreography.SetLedPalette(Led.Left, 1, 1);
// delay: 1, OpCode: SetLedoff Led: Bottom
choreography.SetLedOff(Led.Bottom, 1);
// delay: 0, OpCode: SetLedoff Led: Left
choreography.SetLedOff(Led.Left);
// delay: 0, OpCode: SetLedPalette Led: Center, palette: 1
choreography.SetLedPalette(Led.Center, 1);
// delay: 0, OpCode: SetLedPalette Led: Nose, palette: 2
choreography.SetLedPalette(Led.Nose, 2);
// delay: 1, OpCode: SetLedoff Led: Center
choreography.SetLedOff(Led.Center, 1);
// delay: 0, OpCode: SetLedPalette Led: Right, palette: 1
choreography.SetLedPalette(Led.Right, 1);
// delay: 1, OpCode: SetLedoff Led: Right
choreography.SetLedOff(Led.Right, 1);
// delay: 0, OpCode: SetLedPalette Led: Bottom, palette: 0
choreography.SetLedPalette(Led.Bottom, 0);
// delay: 1, OpCode: SetLedoff Led: Nose
choreography.SetLedOff(Led.Nose, 1);
```

The definition of the Set functions offers the possibility to include the initial delay, by default the value is 0.

This will create a byte array that can be retrieved, saved to a file or serialized in Base64 to send to the pynab service. The following example serialize the previous example and send it to play including a sound (from the nabsurprised files).

```csharp
 // Playing a Choreography and streaming music
Sequence[] seq = new Sequence[] { new Sequence(), new Sequence() };
//seq.ChoreographyList = new string[] { CreateChoreography().SerializeChoreography() };
seq[0].ChoreographyList = CreateChoreography().SerializeChoreography();
seq[1].AudioList = new string[] { "nabsurprised/respirations/Respiration01.mp3" };
// set this one with a timeout
var resp = _nabaztag.Command(seq, true, 30);
if (resp.Status == Status.Ok)
    Console.WriteLine("Your Nabaztag is doing a respiration and a small choreography");
else
    Console.WriteLine($"Something wrong happened: {resp.ErrorClass}, {resp.ErrorMessage}");
```

To save your Choreography into a file, just use a file name :

```csharp
choreography.SaveToFile("filetosave.chor");
```

To load a predefined Choreography:

```csharp
Choreography choreography = new Choreography("filetoload.chor");
```

To get the byte array and manipulate it yourself:
```csharp
var myArray = choreography.ToArray();
```

## Simple Text to Speech example using Azure Cognitive Services

You'll find a very simple [Text to Speech example](/Nabaztag.Tts) using Azure Cognitive Services. For low usage, this is free and give a great quality result for almost all languages when using the "neural" languages.

See https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/index-text-to-speech for more information. You'll need an Azure account, you can create one for free.

When selecting your Text to Speech service, sleect F0, this is free, you have limitation but for a normal usage, it should be all enough. Otherwise, price of transactions for a normal usage is few cents of $/€ every month.

The example is a .NET 4.8 application and using mono to run on the Raspberry Pi zero. See the hack about playing [audio files and path](#audio-files-and-path). 

The configuration file looks like this:

```json
{
  "Path": "tts",
  "CognitiveServices": {
    "Key": "yourkey",
    "EndPoint": "https://westeurope.api.cognitive.microsoft.com/sts/v1.0/issueToken",
    "PreferedVoiceShortName": "fr-FR-DeniseNeural"
  }
}
```

The key is the cognitive services key you'll get from the Azure Cognitive Services once you'll create it. The Endpoint is displayed on the page as well. 

The prefered voice is the voice you'll choose. Depending on the region you'll create your service, some voices are not available. There is a function that will allow you to dump in a ```voices.json``` file all the voices. You can then select the one you'd like with the voice you'd like.

Compile it the normal way once in the `Nabaztag.Tts` directory:

```cmd
dotnet restore
dotnet build
```

You'll have to copy into the `~/tts` path or any of the path you'd like in the home directory (specity it the Path confirguration element) all the generated files in `\bin\Debug\net48`(or Release depending how you've build it). Then run:

```bash
mono Nabaztag.Tts.exe
```

You'll get as a result:
```
pi@nabaztag:~/tts $ mono Nabaztag.Tts.exe
Hello Text to Speech!
Type the text for Nabaztag to say: C'est super cool d'avoir un exemple de text to speech qui fonctionne top!
TTS played properly
```

And of course the pleasure, in this case with the French voice selected, to have the Nabaztag reading the text. Note that it takes more time for the Nabaztag to move the ear and start speaking than it takes time to go the Azure Services, download the file and send it to play to the Nabaztag :-)

# TODO

- improve documentation
- add rfid support
- improve the simulator
- add more tests
