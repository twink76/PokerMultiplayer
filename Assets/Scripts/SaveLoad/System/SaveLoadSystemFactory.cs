using UnityEngine;

public class SaveLoadSystemFactory : MonoBehaviour
{
    public static SaveLoadSystemFactory Instance { get; private set; }

    [SerializeField] private SaveLoadSystemType _saveLoadSystemType;

    private ISaveLoadSystem _saveLoadSystem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ISaveLoadSystem Get()
    {
        if (_saveLoadSystem == null)
        {
            switch (_saveLoadSystemType)
            {
                case SaveLoadSystemType.Binary:
                    _saveLoadSystem = new BinarySaveLoadSystem();
                    break;
                case SaveLoadSystemType.MySql:
                    _saveLoadSystem = new MySqlSaveLoadSystem();
                    break;
                default:
                    return null;
            }
        }

        return _saveLoadSystem;
    }
}
