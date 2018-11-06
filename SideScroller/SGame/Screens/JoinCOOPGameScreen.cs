using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG2D.GameEngine;
using RPG2D.GameEngine.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.SGame.Screens
{
    public class JoinCOOPGameScreen : IGameScreen
    {
        public string ip = "";
        public string messageLog = "";
        public bool showingDiag = false;
        Texture2D titleTexture;

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameManager.Game.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            float titleTexX = GameEngine.GameManager.Game.ScreenSize.X / 2 - titleTexture.Width / 2;
            spriteBatch.Draw(titleTexture, new Vector2(titleTexX, 2), Color.White);
            spriteBatch.DrawString(GlobalAssets.Arial24, "IP: " + ip, new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString("IP: " + ip).X / 2, 200), Color.Black);
            spriteBatch.DrawString(GlobalAssets.Arial24, messageLog.ToString(), new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString(messageLog).X / 2, 250), Color.DarkGray);

            spriteBatch.End();
        }

        public void Init(ContentManager content)
        {
            titleTexture = content.Load<Texture2D>("title");

        }
        NetClient client;
        bool polling = false;
        public void Update(GameTime gameTime)
        {
            if (showingDiag == false)
            {
                showingDiag = true;
                ip = KeyboardInput.Show("Server IP", "Enter the ip adress of the server host.", "127.0.0.1").Result;
                Console.WriteLine("Begining server handshake protocol");
                System.Threading.Thread.Sleep(1000);

                if (ip != null)
                {
                    var config = new NetPeerConfiguration("RPG2D");
                    client = new NetClient(config);
                    client.Start();
                    client.Connect(ip, 20666);
                    System.Threading.Thread.Sleep(1000);
                    Console.WriteLine("Polling for handshake response...");
                    polling = true;
                }
            }
            if (polling)
            {
                polling = false;
                client.SendMessage(client.CreateMessage("handshake(Player2,1)"), NetDeliveryMethod.ReliableOrdered);
                client.FlushSendQueue();
                Console.WriteLine("Request server handshake...");

                NetIncomingMessage msg;
                string msgTxt = "";
                System.Threading.Thread.Sleep(100);

                bool foundServer = false;
                string name="";
                while (!foundServer)
                {
                    msg = client.ReadMessage();

                    if (msg == null)
                        continue;

                    if (msg.MessageType == NetIncomingMessageType.Data)
                    {
                        msgTxt = msg.ReadString();
                        if (msgTxt.StartsWith("handshake"))
                        {
                            name = msgTxt.Split('(', ')')[1].Split(',')[0];
                            
                            Console.WriteLine("Handshake complete, joing user " + name + " after map download");
                            client.SendMessage(client.CreateMessage("world()"), NetDeliveryMethod.ReliableOrdered);

                        }
                        if (msgTxt.StartsWith("world"))
                        {
                            Console.WriteLine("World recieved");
                            string xml = msgTxt.Split('(', ')')[1];
                            client.SendMessage(client.CreateMessage("connectioncomplete()"), NetDeliveryMethod.ReliableOrdered);
                            client.Disconnect("");
                            foundServer = true;
                            GameManager.Game.ChangeScreen(new COOPGameScreen(ip, name, xml));
                        }
                    }

                }
            }
        }
    }
}
