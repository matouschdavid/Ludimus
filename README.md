# Ludimus

Ludimus is a mobile gaming plattform for all sorts of card-, board- and arcade games designed for the holiday of the modern family.

We like to open our plattform to everyone who wants to get involved and has a passion for game development.

# Get started
To get started just clone this template project and follow the instructions below to create your very first Ludimus game.

Games in Ludimus always consist of two parts. A server and a client application. You can develop both parts in the same Unityproject. Just create a subfolder under Ludimus/Games/ . You can have many scenes in your game but the project has to have atleast two scenes that function as a starting point.
These scenes must follow this naming convention:

"gamename"_Server

"gamename"_Client
 
Even tho we do not intend to constrain our developers, we recommend the following structure:
 
Use the client for player input and displaying private information and the server for the boardtable, the gameworld or such and for showing information that´s interessting for every player.


# Client

Lets start with the client. The client can send and receive messages and we provide you easy access to both operations.

## Writing Data
To write Data you only have to call the static method ConnectionController.Write()
All informations between server and client consist of a key and a value. The write method can take two string parameters, the first for the key the second for the value or a data object. Both the client and the server use the same method for writing. The Connection parameter can and should be ignored when writing as a client.
```cs
            ConnectionController.Write("That's my key", "That's my value");
            
            //or
            
            var data = new Data
            {
                Key = "That's my key",
                Value = "That's my value"
            };
            ConnectionController.Write(data);
```

## Receiving Data
To receive messages you have to attach a callback function. But first you need to get a reference to the client.
```cs
        void Start()
        {
            client = ConnectionController.GetControllerInstance<ClientConnection>();
            client.AttachMessageHandler(MessageCallback);
        }

        private void MessageCallback(Data data, Connection connection)
        {
            ...
        }
```

The connection Parameter points to the client, but we don´t need it as a client. The MessageCallback function is called for every new message the client receives.


# Server
The server is a bit more complicated and possibly subject to change.

## Writing Data
The server can write messages to one or every player. The syntax for writing to every player is the exact same as writing as a client.
        ConnectionController.Write("My key", "my value");
To write to one specific player you need to have his connection object stored. Then you can write him a message with:
```cs
        ConnectionController.Write(myPlayer, "my key", "my value");
 ```     
 
## Receiving Data
The server can receive single messages just like the client.
```cs
        server = ConnectionController.GetControllerInstance<ServerConnection>();
        server.AttachMessageHandler(MessageCallback);
```     
But the server can also wait until some amount of players wrote the same message. Usage examples are waiting until every player is ready or until every player has answered the question (in a quiz game).
Doing this is very easy.
```cs
        server.WaitForGroupAction("the key you look for", "a value you want send with you", TheCallbackWhenEveryPlayerHasSentTheKey);
 ```     
There is an optional fourth parameter that lets you specify how many players have to send the message. For example in a quiz game only the four quickest answers are important. By default every player has to send the message. You still get every message the normal way too.



# Starting and testing your game
We've implemented a few helpers to speed up your development process. To quickly debug your game in the editor, go under

Ludimus->Start in Editor->Server/Client

The correct scene gets selected and the game automaticly starts. To test your app you most likely have to have two instances of Ludimus open at the same time. Our recommendation is running the part you wanna debug in the editor and the other as a built application. To Quickly build your app for android and windows go under

Ludimus->Build->Server/Client->Android/Windows

When building for android please keep in mind that the apk does not get installed on your connected device. Currently you have to drag and drop the built apk onto your device yourself.
