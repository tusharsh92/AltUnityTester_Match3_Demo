
using System.Threading;
using UnityEngine;
using NUnit.Framework;
using System.Linq;


[Timeout(5000)]

public class TC1
{
    public AltUnityDriver AltUnityDriver;
    //Before any test it connects with the socket
    [OneTimeSetUp]
    public void SetUp()
    {
        AltUnityDriver = new AltUnityDriver();
    }

    //At the end of the test closes the connection with the socket
    [OneTimeTearDown]
    public void TearDown()
    {
        AltUnityDriver.Stop();
    }


    [Test, Order(1)]
    public void TestGetCurrentScene()
    {
       Assert.AreEqual("MainMenu", AltUnityDriver.GetCurrentScene());

    }


    [Test, Order(2)]
    public void TestPlayButton()
    {

        var playButton = AltUnityDriver.FindElement("MainMenuObjects/Play_Main_Menu", "Main Camera");

        AltUnityDriver.HoldButton(new Vector2(playButton.x, playButton.y), 1);
        
        Thread.Sleep(6000);
    }

    [Test, Order(3)]
    public void TestGetCurrentScene2()
    {
        Assert.AreEqual("Game", AltUnityDriver.GetCurrentScene());

    }

    [Test, Order(4)]
    public void TestBombSetOff()
    {
        var bombs = AltUnityDriver.FindElementsWhereNameContains("Bomb");
        AltUnityDriver.HoldButton(new Vector2(bombs[0].x, bombs[0].y), 1);
        Thread.Sleep(3000);
    }

    [Test, Order(5)]
    public void TestFlamesCount()
    {
        var bomb = AltUnityDriver.FindElementWhereNameContains("Bomb");
        AltUnityDriver.HoldButton(new Vector2(bomb.x, bomb.y), 1);

        var flames = AltUnityDriver.FindElementsWhereNameContains("Flame");
        Assert.AreEqual(19, flames.Count());

        Thread.Sleep(3000);
    }

    [Test, Order(6)]
    public void TestPauseButton()
    {


        AltUnityDriver.WaitForCurrentSceneToBe("Game");
        AltUnityDriver.WaitForElement("HUD/PauseButon");

        var pauseButton = AltUnityDriver.FindElement("HUD/PauseButon");
        
        AltUnityDriver.HoldButton(new Vector2(pauseButton.x, pauseButton.y), 1);

        Thread.Sleep(3000);      
       
    }

        
}