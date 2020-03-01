# Nabaztag.Net to have a full interaction with the pynab service

The aim of this project is to fully interact with the [pynab service](https://github.com/nabaztag2018/pynab).

This library is under development. As you can't deploy .NET Core application on the Raspeberry Pi zero, you need to deploy it somewhere on your network and make sure your rabbit will listen to request coming from outside of it's body :-) See [here how setup the connection](https://github.com/nabaztag2018/pynab/issues/101)

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
Sequence seq = new Sequence();
seq.ChoreographyList = new string[] { CreateChoreography().SerializeChoreography() };
seq.AudioList = new string[] { "nabsurprised/respirations/Respiration01.mp3" };
// set this one with a timeout
resp = nabaztag.Command(seq, true, 30);
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



# TODO

- create documentation
- add more interaction with pynab (so far onlye sleep and wake up available)
- improve the simulator
- add more tests
