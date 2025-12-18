using NUnit.Framework;
using UnityEngine;

public class GameManagerTests
{
    private GameObject gameManagerObj;
    private GameManager gameManager;

    // Этот метод запускается перед КАЖДЫМ тестом
    [SetUp]
    public void SetUp()
    {
        // Создаём виртуальный объект GameManager для тестов
        gameManagerObj = new GameObject("Test_GameManager");
        gameManager = gameManagerObj.AddComponent<GameManager>();
        // Т.к. GameManager использует Singleton (Instance), присваиваем его
        // В реальном коде лучше избегать синглтонов для тестируемости
        System.Reflection.FieldInfo instanceField = typeof(GameManager).GetField("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        instanceField.SetValue(null, gameManager);
    }

    [TearDown]
    public void TearDown()
    {
        // Уничтожаем объект после каждого теста
        Object.DestroyImmediate(gameManagerObj);
    }

    // ТЕСТ 1: Инициализация счёта
    [Test]
    public void GameManager_InitialScore_IsZero()
    {
        Assert.AreEqual(0, gameManager.GetCurrentScore());
    }

    // ТЕСТ 2: Метод AddScore корректно увеличивает счёт
    [Test]
    public void GameManager_AddScore_IncreasesTotalScore()
    {
        gameManager.AddScore(5);
        gameManager.AddScore(3);

        Assert.AreEqual(8, gameManager.GetCurrentScore());
    }

    // ТЕСТ 3: Проверка граничного случая - добавление 0 очков
    [Test]
    public void GameManager_AddZeroScore_ScoreUnchanged()
    {
        gameManager.AddScore(0);
        Assert.AreEqual(0, gameManager.GetCurrentScore());
    }

    // ТЕСТ 4: Проверка победы в начале игры
    [Test]
    public void GameManager_WinCondition_IsFalseAtStart()
    {
        Assert.IsFalse(gameManager.IsWinConditionMet);
    }

    // ТЕСТ 5: Проверка победы при сборе всех монет
    [Test]
    public void GameManager_WinCondition_BecomesTrueWhenAllCoinsCollected()
    {
        // Собираем 3 монеты - победа ещё не достигнута
        gameManager.AddScore(3);
        Assert.IsFalse(gameManager.IsWinConditionMet);

        // Собираем 4-ю монету - победа достигнута
        gameManager.AddScore(1);
        Assert.IsTrue(gameManager.IsWinConditionMet);
    }

    // ТЕСТ 6: Проверка обнуления счётчика при рестарте игры
    [Test]
    public void GameManager_ResetGame_ResetsScoreAndWinCondition()
    {
        gameManager.AddScore(4);
        Assert.IsTrue(gameManager.IsWinConditionMet);
        Assert.AreEqual(4, gameManager.GetCurrentScore());

        gameManager.ResetGame();
        Assert.IsFalse(gameManager.IsWinConditionMet);
        Assert.AreEqual(0, gameManager.GetCurrentScore());
    }
}