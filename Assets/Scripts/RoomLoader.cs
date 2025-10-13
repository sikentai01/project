using UnityEngine;
using UnityEngine.SceneManagement;

public static class RoomLoader
{
    static string _currentRoom;

    public static void LoadRoom(string nextRoomSceneName, string spawnName)
    {
        if (string.IsNullOrEmpty(nextRoomSceneName))
        {
            Debug.LogWarning("[RoomLoader] nextRoomSceneName が空です");
            return;
        }

        LevelSpawnRouter2D.NextSpawnPointName =
            string.IsNullOrEmpty(spawnName) ? "SpawnPoint" : spawnName;

        if (!string.IsNullOrEmpty(_currentRoom) && _currentRoom == nextRoomSceneName)
            return;

        SceneManager.LoadSceneAsync(nextRoomSceneName, LoadSceneMode.Additive)
            .completed += _ =>
            {
                var next = SceneManager.GetSceneByName(nextRoomSceneName);
                if (next.IsValid()) SceneManager.SetActiveScene(next);

                if (!string.IsNullOrEmpty(_currentRoom))
                {
                    var old = SceneManager.GetSceneByName(_currentRoom);
                    if (old.IsValid() && old.isLoaded) SceneManager.UnloadSceneAsync(old);
                }
                _currentRoom = nextRoomSceneName;
            };
    }
}
