using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;

    public enum ListOfSound
    {
        nbSound
    }


    [SerializeField] int sizePoolSound = 10;
    [SerializeField] List<string> keysMap;
    [SerializeField] List<AudioClip> valuesMap;
    Dictionary<string, List<AudioClip>> bank;

    AudioSource[] pool;
    AudioSource music;

    bool musicLoop;
    string musicName;


    #region assessors
    public static SoundManager GetSoundManager
    {
        get
        {
            return instance;
        }
    }
    #endregion

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        musicLoop = false;
        musicName = "";
        InitDictionary();
        InitPool(sizePoolSound);
        music = gameObject.AddComponent<AudioSource>();
        PlayMusic("MenuMusic", 0.1f, true);
    }

    void InitDictionary()
    {
        bank = new Dictionary<string, List<AudioClip>>();
        for (int i = 0; i < keysMap.Count; i++)
        {
            if (!bank.ContainsKey(keysMap[i]))
            {
                List<AudioClip> audioClips = new List<AudioClip>();
                bank.Add(keysMap[i], audioClips);
            }
            bank[keysMap[i]].Add(valuesMap[i]);
        }
    }

    public void AddDictionary(List<string> keysMap, List<AudioClip> valuesMap)
    {
        for (int i = 0; i < keysMap.Count; i++)
        {
            if (!bank.ContainsKey(keysMap[i]))
            {
                List<AudioClip> audioClips = new List<AudioClip>();
                bank.Add(keysMap[i], audioClips);
            }
            bank[keysMap[i]].Add(valuesMap[i]);
        }
    }

    public void RemoveDictionary(List<string> keysMap)
    {
        for (int i = 0; i < keysMap.Count; i++)
        {
            bank.Remove(keysMap[i]);
        }
    }

    void InitPool(int sizePool)
    {
        pool = new AudioSource[sizePool];
        for (int i = 0; i < sizePool; i++)
        {
            pool[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    public bool AddSound(string key, AudioClip clip)
    {
        if (!bank.ContainsKey(key))
        {
            List<AudioClip> audioClips = new List<AudioClip>();
            bank.Add(key, audioClips);
        }
        bank[key].Add(clip);
        return true;
    }

    public void StopAllSound()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i].isPlaying)
            {
                pool[i].Stop();
            }
        }
    }

    public bool StopSound(string nameSound)
    {
        bool haveStopSound = false;
        if (bank[nameSound] == null)
        {
            return false;
        }
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i].isPlaying && bank[nameSound].Contains(pool[i].clip))
            {
                pool[i].Stop();
                haveStopSound = true;
            }
        }
        return haveStopSound;
    }

    public bool PlaySound(string nameSound)
    {
        if (bank[nameSound] == null)
        {
            return false;
        }
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].isPlaying)
            {
                pool[i].clip = bank[nameSound][Random.Range(0, bank[nameSound].Count)];
                pool[i].volume = 1f;
                pool[i].pitch = 1f;
                pool[i].time = 0f;
                pool[i].Play();
                return true;
            }
        }
        return false;
    }

    public bool PlaySound(string nameSound, float volume)
    {
        if (bank[nameSound] == null)
        {
            return false;
        }
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].isPlaying)
            {
                pool[i].clip = bank[nameSound][Random.Range(0, bank[nameSound].Count)];
                pool[i].volume = volume;
                pool[i].pitch = 1f;
                pool[i].time = 0f;
                pool[i].Play();
                return true;
            }
        }
        return false;
    }

    public bool PlaySound(string nameSound, float volume, float pitch)
    {
        if (bank[nameSound] == null)
        {
            return false;
        }
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].isPlaying)
            {
                pool[i].clip = bank[nameSound][Random.Range(0, bank[nameSound].Count)];
                pool[i].volume = volume;
                pool[i].pitch = pitch;
                pool[i].time = 0f;
                pool[i].Play();
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (musicLoop && !music.isPlaying)
        {
            PlayMusic(musicName, music.volume, musicLoop);
        }
    }

    public bool PlayMusic(string nameSound)
    {
        if (bank[nameSound] == null)
        {
            return false;
        }
        musicName = nameSound;
        music.clip = bank[nameSound][Random.Range(0, bank[nameSound].Count)];
        music.volume = 1f;
        music.loop = false;
        musicLoop = false;
        music.time = 0f;
        music.Play();
        return true;
    }

    public bool PlayMusic(string nameSound, float volume)
    {
        if (bank[nameSound] == null)
        {
            return false;
        }
        musicName = nameSound;
        music.clip = bank[nameSound][Random.Range(0, bank[nameSound].Count)];
        music.volume = volume;
        music.loop = false;
        musicLoop = false;
        music.time = 0f;
        music.Play();
        return true;
    }

    public bool PlayMusic(string nameSound, bool loop)
    {
        if (bank[nameSound] == null)
        {
            return false;
        }
        musicName = nameSound;
        music.clip = bank[nameSound][Random.Range(0, bank[nameSound].Count)];
        music.volume = 1f;
        music.loop = false;
        musicLoop = loop;
        music.time = 0f;
        music.Play();
        return true;
    }

    public bool PlayMusic(string nameSound, float volume, bool loop)
    {
        if (bank[nameSound] == null)
        {
            return false;
        }
        musicName = nameSound;
        music.clip = bank[nameSound][Random.Range(0, bank[nameSound].Count)];
        music.volume = volume;
        music.loop = false;
        musicLoop = loop;
        music.time = 0f;
        music.Play();
        return true;
    }

    public void ChangeMusicByScene(int builtIndex)
    {
        if (instance != this)
        {
            return;
        }
        Scene scene = SceneManager.GetSceneByBuildIndex(builtIndex);
        string sceneName = scene.name;

        if ((sceneName == "SelectionClass" || sceneName == "EditorCharacter" || sceneName == "SelectionSpecialPlayer"))
        {
            if (!bank["CreationPersoMusic"].Contains(music.clip))
            {
                PlayMusic("CreationPersoMusic", 0.1f, true);
            }
        }
        else if (sceneName == "MainScreen")
        {
            if (!bank["MenuMusic"].Contains(music.clip))
            {
                PlayMusic("MenuMusic", 0.1f, true);
            }
        }
        else if (sceneName.Contains("Interrieur"))
        {
            if (!bank["House"].Contains(music.clip))
            {
                PlayMusic("House", 0.1f, true);
            }
        }
        else if (sceneName.Contains("Grotte"))
        {
            if (!bank["Grotte"].Contains(music.clip))
            {
                PlayMusic("Grotte", 0.1f, true);
            }
        }
        else if (sceneName == "FightingMap")
        {
            if (!bank["Smourbif"].Contains(music.clip))
            {
                PlayMusic("Smourbif", 0.1f, true);
            }
        }
        else if (builtIndex > 2)
        {
            if (!bank["MainMusic"].Contains(music.clip))
            {
                PlayMusic("MainMusic", 0.1f, true);
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        ChangeMusicByScene(level);
    }

    public bool MusicIsPlaying
    {
        get
        {
            return music.isPlaying;
        }
    }

    public void StopMusic()
    {
        if (music != null)
        {
            music.Stop();
        }
    }

    public void PauseMusic()
    {
        music.Pause();
    }

    public void UnpauseMusic()
    {
        music.Play();
    }
}
