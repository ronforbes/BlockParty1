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
            Pregame,
            Gameplay,
        }
        GameState state = GameState.Pregame;
        TimeSpan pregameElapsed;
        TimeSpan pregameDuration = TimeSpan.FromSeconds(10);
        TimeSpan roundResultsElapsed;
        TimeSpan roundResultsDuration = TimeSpan.FromSeconds(5);
        bool shownRoundResults;
        TimeSpan gameplayElapsed;
        TimeSpan gameplayDuration = TimeSpan.FromSeconds(13);

        Timer updateTimer;

        const int updatesPerSecond = 1;

        GameTime gameTime = new GameTime();

        NetworkingManager networkingManager = new NetworkingManager();

        public Dictionary<string, int> RoundResults = new Dictionary<string, int>();

        public Game()
        {
            networkingManager.Game = this;

            updateTimer = new Timer(1000.0 / updatesPerSecond);
            updateTimer.Elapsed += Update;
            updateTimer.Start();

            while (true)
            {

            }
        }

        void Update(object sender, ElapsedEventArgs e)
        {
            gameTime.Update();

            switch(state)
            {
                case GameState.Pregame:
                    pregameElapsed += gameTime.Elapsed;
                    roundResultsElapsed += gameTime.Elapsed;

                    if(roundResultsElapsed >= roundResultsDuration)
                    {
                        if(!shownRoundResults)
                        {
                            List<KeyValuePair<string, int>> ranking = RoundResults.ToList();

                            ranking.Sort((firstPair, nextPair) =>
                                {
                                    return firstPair.Value.CompareTo(nextPair.Value) * -1;
                                });

                            if(ranking.Count > 0)
                            {
                                Console.WriteLine("The winner is " + ranking[0].Key + " with a score of " + ranking[0].Value);
                                NetworkMessage message = new NetworkMessage();
                                message.Type = NetworkMessage.MessageType.RoundResults;
                                message.Content = ranking;
                                networkingManager.BroadcastData(message);
                            }

                            shownRoundResults = true;
                        }
                    }

                    if(pregameElapsed >= pregameDuration)
                    {
                        Console.WriteLine("Starting gameplay");

                        NetworkMessage message = new NetworkMessage();
                        message.Type = NetworkMessage.MessageType.GameState;
                        message.Content = "Gameplay";
                        networkingManager.BroadcastData(message);

                        state = GameState.Gameplay;
                        gameplayElapsed = TimeSpan.Zero;
                    }
                    break;

                case GameState.Gameplay:
                    gameplayElapsed += gameTime.Elapsed;

                    if(gameplayElapsed >= gameplayDuration)
                    {
                        Console.WriteLine("Starting pregame");

                        RoundResults.Clear();

                        NetworkMessage message = new NetworkMessage();
                        message.Type = NetworkMessage.MessageType.GameState;
                        message.Content = "Pregame";
                        networkingManager.BroadcastData(message);

                        state = GameState.Pregame;
                        pregameElapsed = TimeSpan.Zero;
                        roundResultsElapsed = TimeSpan.Zero;

                        shownRoundResults = false;
                    }

                    break;
            }
        }
    }
}