using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameplayIntegrationTests
{
    private GameObject player;
    private GameManager gameManager;
    private GameObject[] coins;

    // Первый тест: игрок собирает одну монету
    [UnityTest]
    public IEnumerator Player_CollectsCoin_ScoreIncreases()
    {
        CreateTestEnvironment(1);
    
        yield return new WaitForFixedUpdate();

        player.transform.position = coins[0].transform.position;

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.AreEqual(1, gameManager.GetCurrentScore(), "Счёт должен увеличиться на 1 после сбора монеты");

        Assert.IsTrue(gameManager.GetCurrentScore() > 0, "Счет должен быть больше 0 после сбора монеты");

        CleanupTestEnvironment();
    }

    // ВТОРОЙ ТЕСТ: Проверка условия победы (когда собраны все монеты)
    [UnityTest]
    public IEnumerator GameManager_AllCoinsCollected_WinConditionMet()
    {
        // 1. ARRANGE - Создаём 4 монеты (как в GameManager.totalCoins)
        CreateTestEnvironment(4);
        
        // Запоминаем начальный счёт для отладки
        int startScore = gameManager.GetCurrentScore();
        Debug.Log($"Начальный счёт: {startScore}");
        
        // 2. ACT - Собираем все монеты одну за другой
        for (int i = 0; i < coins.Length; i++)
        {
            if (coins[i] != null)
            {
                player.transform.position = coins[i].transform.position;
                yield return new WaitForSeconds(0.1f); // Небольшая задержка
                
                Debug.Log($"Собрана монета {i + 1}, текущий счёт: {gameManager.GetCurrentScore()}");
            }
        }
        
        // 3. ASSERT - Проверяем, что счёт равен количеству монет
        Assert.AreEqual(4, gameManager.GetCurrentScore(), $"Счёт должен быть равен {coins.Length}");
        
        // 4. ASSERT - Проверяем, что условие победы сработало
        Assert.IsTrue(gameManager.IsWinConditionMet, "Должно сработать условие победы");
        
        // 5. Cleanup
        CleanupTestEnvironment();
    }
    
    // ТРЕТИЙ ТЕСТ: Проверка уничтожения монеты после сбора
    [UnityTest]
    public IEnumerator Coin_IsDestroyed_AfterCollection()
    {
        // 1. ARRANGE
        CreateTestEnvironment(1);
        var coinObject = coins[0];
        
        // Сохраняем имя объекта для проверки
        string coinName = coinObject.name;
        
        // 2. ACT
        player.transform.position = coinObject.transform.position;
        yield return new WaitForSeconds(0.2f);
        
        // 3. ASSERT - Проверяем, что монета уничтожена
        // Ищем объект с таким именем
        GameObject foundCoin = GameObject.Find(coinName);
        Assert.IsNull(foundCoin, $"Монета {coinName} должна быть уничтожена после сбора");
        
        // 4. Cleanup
        CleanupTestEnvironment();
    }
    
    // Вспомогательный метод для создания тестовой среды
    private void CreateTestEnvironment(int coinCount)
    {
        // Создаём игрока БЕЗ примитива (чтобы не было MeshCollider)
        player = new GameObject("TestPlayer");
        
        // Добавляем Rigidbody2D ПЕРВЫМ (важный порядок!)
        var playerRb = player.AddComponent<Rigidbody2D>();
        playerRb.gravityScale = 0;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll; // Замораживаем все
        playerRb.isKinematic = true;
        
        // Добавляем BoxCollider2D (2D коллайдер)
        var playerCollider = player.AddComponent<BoxCollider2D>();
        playerCollider.isTrigger = true;
        playerCollider.size = Vector2.one * 1.0f;
        
        player.tag = "Player";
        player.transform.position = Vector3.zero;
        
        // Создаём GameManager
        var gameManagerObj = new GameObject("Test_GameManager");
        gameManager = gameManagerObj.AddComponent<GameManager>();
        
        // Устанавливаем Instance через reflection
        var instanceField = typeof(GameManager).GetField("Instance", 
            System.Reflection.BindingFlags.Static | 
            System.Reflection.BindingFlags.Public);
        if (instanceField != null)
        {
            instanceField.SetValue(null, gameManager);
        }
        
        // Создаём монеты
        coins = new GameObject[coinCount];
        for (int i = 0; i < coinCount; i++)
        {
            // Создаём монету БЕЗ примитива
            coins[i] = new GameObject($"TestCoin_{i}");
            coins[i].transform.position = new Vector3((i + 1) * 2, 0, 0); // Размещаем по горизонтали
            
            // Добавляем компонент Coin
            var coinScript = coins[i].AddComponent<Coin>();
            coinScript.scoreValue = 1;
            
            // Добавляем CircleCollider2D с триггером
            var collider = coins[i].AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }
        
        // Для визуализации в редакторе можно добавить SpriteRenderer (опционально)
        #if UNITY_EDITOR
        AddDebugVisuals();
        #endif
    }
    
    // Добавляем визуализацию для отладки в редакторе
    private void AddDebugVisuals()
    {
        // Добавляем спрайт игроку
        var playerSprite = player.AddComponent<SpriteRenderer>();
        playerSprite.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        playerSprite.color = Color.blue;
        
        // Добавляем спрайты монетам
        foreach (var coin in coins)
        {
            var coinSprite = coin.AddComponent<SpriteRenderer>();
            coinSprite.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            coinSprite.color = Color.yellow;
        }
    }
    
    // Вспомогательный метод для очистки
    private void CleanupTestEnvironment()
    {
        if (player != null) 
        {
            Object.DestroyImmediate(player);
        }
        
        var gameManagerObj = GameObject.Find("Test_GameManager");
        if (gameManagerObj != null) 
        {
            Object.DestroyImmediate(gameManagerObj);
        }
        
        if (coins != null)
        {
            foreach (var coin in coins)
            {
                if (coin != null) 
                {
                    Object.DestroyImmediate(coin);
                }
            }
        }
        
        // Сбрасываем GameManager.Instance
        var instanceField = typeof(GameManager).GetField("Instance", 
            System.Reflection.BindingFlags.Static | 
            System.Reflection.BindingFlags.Public);
        if (instanceField != null)
        {
            instanceField.SetValue(null, null);
        }
    }
}