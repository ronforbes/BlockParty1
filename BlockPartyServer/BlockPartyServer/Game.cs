using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BlockPartyShared;

namespace BlockPartyServer
{
    public class Game
    {
        enum GameState
        {
            Lobby,
            Game,
        }

        GameState state = GameState.Lobby;
        TimeSpan lobbyElapsed;
        TimeSpan lobbyDuration = TimeSpan.FromSeconds(10);
        TimeSpan gameResultsElapsed;
        TimeSpan gameResultsDuration = TimeSpan.FromSeconds(5);
        bool shownGameResults;
        TimeSpan gameElapsed;
        TimeSpan gameDuration = TimeSpan.FromSeconds(13);

        Timer updateTimer;

        const int updatesPerSecond = 1;

        GameTime gameTime = new GameTime();

        NetworkingManager networkingManager = new NetworkingManager();

        public Dictionary<string, int> GameResults = new Dictionary<string, int>();

        public Game()
        {
            networkingManager.Game = this;
            networkingManager.MessageReceived += networkingManager_MessageReceived;
            updateTimer = new Timer(1000.0 / updatesPerSecond);
            updateTimer.Elapsed += Update;
            updateTimer.Start();

            while(true)
            {

            }
        }

        void networkingManager_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch(e.Message.Type)
            {
                case NetworkMessage.MessageType.ClientGameResults:
                    GameResults.Add(e.Sender, (int)e.Message.Content);
                    break;
            }
        }

        void Update(object sender, ElapsedEventArgs e)
        {
            gameTime.Update();

            switch(state)
            {
                case GameState.Lobby:
                    lobbyElapsed += gameTime.Elapsed;
                    gameResultsElapsed += gameTime.Elapsed;

                    if(gameResultsElapsed >= gameResultsDuration)
                    {
                        if(!shownGameResults)
                        {
                            List<KeyValuePair<string, int>> ranking = GameResults.ToList();

                            ranking.Sort((firstPair, nextPair) =>
                                {
                                    return firstPair.Value.CompareTo(nextPair.Value) * -1;
                                });

                            if(ranking.Count > 0)
                            {
                                Console.WriteLine("The winner is " + ranking[0].Key + " with a score of " + ranking[0].Value);
                                NetworkMessage message = new NetworkMessage();
                                message.Type = NetworkMessage.MessageType.ServerGameResults;
                                message.Content = ranking;
                                networkingManager.Broadcast(message);
                            }

                            shownGameResults = true;
                        }
                    }

                    if(lobbyElapsed >= lobbyDuration)
                    {
                        Console.WriteLine("Starting game");

                        NetworkMessage message = new NetworkMessage();
                        message.Type = NetworkMessage.MessageType.ServerGameState;
                        message.Content = "Game";
                        networkingManager.Broadcast(message);

                        state = GameState.Game;
                        gameElapsed = TimeSpan.Zero;
                    }
                    break;

                case GameState.Game:
                    gameElapsed += gameTime.Elapsed;

                    if(gameElapsed >= gameDuration)
                    {
                        Console.WriteLine("Starting lobby");

                        GameResults.Clear();

                        NetworkMessage message = new NetworkMessage();
                        message.Type = NetworkMessage.MessageType.ServerGameState;
                        message.Content = "Lobby";
                        networkingManager.Broadcast(message);

                        state = GameState.Lobby;
                        lobbyElapsed = TimeSpan.Zero;
                        gameResultsElapsed = TimeSpan.Zero;

                        shownGameResults = false;
                    }

                    break;
            }
        }
    }
}