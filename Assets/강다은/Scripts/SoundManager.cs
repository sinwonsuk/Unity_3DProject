using UnityEngine;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
	public enum sfx
	{
		itemUpgrade,
		itemDrop,
		buyItem,
        subtractGold,
		BuyComp,
		StoreOpen,
		StoreClose,
	}
	public enum bgm
	{

	}
	private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
		Init();
	}

	public void SfxPlay(sfx sfx, bool _loopcheck, float volume = 0.5f)
	{
		if (sfxClips[(int)sfx] == null)
		{
			Debug.LogWarning($"SFX 클립이 없습니다: {sfx}");
			return;
		}

		for (int i = 0; i < sfxSound.Length; i++)
		{
			if (sfxSound[i].isPlaying)
			{
				continue;
			}

			sfxSound[i].clip = sfxClips[(int)sfx];
			sfxSound[i].Play();
			//sfxSound[i].volume = volume > 0 ? volume : sfxVolume;
			//sfxSound[i].volume = volume;
			sfxSound[i].volume = sfxVolume; // 항상 내부 설정값 사용

			if (_loopcheck == true)
			{
				sfxSound[i].loop = true;
			}
			else
			{
				sfxSound[i].loop = false;
			}
			break;
		}

	}

	public void UpdateSfxVolumes()
	{
		foreach (var src in sfxSound)
		{
			if (src != null && src.isPlaying)
				src.volume = sfxVolume;
		}
	}


	public void Sfx_Stop(sfx _sfx)
	{
		for (int i = 0; i < sfxSound.Length; i++)
		{
			if (sfxSound[i].clip == sfxClips[(int)_sfx])
			{
				sfxSound[i].Stop();
			}
		}
	}
	public void PlayBgm(bgm _bgm)
	{
		bgmPlayer.clip = bgmClips[(int)_bgm];
		bgmPlayer.Play();
	}
	public void Bgm_Stop()
	{
		bgmPlayer.Stop();
	}
	public void All_Sfx_Stop()
	{
		for (int i = 0; i < sfxSound.Length; i++)
		{
			sfxSound[i].Stop();
		}
	}


	// 발소리, 총소리
	public void Play3DSfx(sfx sfxType, Vector3 position, float volume = 1f)
	{
		if (sfxClips[(int)sfxType] == null)
		{
			Debug.LogWarning($"SFX 클립이 없습니다: {sfxType}");
			return;
		}

		GameObject go = new GameObject($"SFX_{sfxType}_3D");
		go.transform.position = position;

		AudioSource src = go.AddComponent<AudioSource>();
		src.clip = sfxClips[(int)sfxType];
		src.volume = volume * sfxVolume;
		src.spatialBlend = 1f; // 완전한 3D 사운드
		src.minDistance = 1f;
		src.maxDistance = 30f;
		src.rolloffMode = AudioRolloffMode.Logarithmic;

		src.Play();
		Destroy(go, src.clip.length); // 소리 끝나면 삭제
	}

	// 혹시 몰라서 만든 건데
	// 지속적으로 따라다녀야 하는 소리가 필요하다면 이거 쓰면 됨
	public void Play3DSfxWithTransform(sfx sfxType, Vector3 position, float volume = 1f)
	{
		if (sfxClips[(int)sfxType] == null)
		{
			Debug.LogWarning($"SFX 클립이 없습니다: {sfxType}");
			return;
		}

		GameObject go = new GameObject($"SFX_{sfxType}_3D");
		go.transform.position = position;

		AudioSource src = go.AddComponent<AudioSource>();
		src.clip = sfxClips[(int)sfxType];
		src.volume = volume * sfxVolume;
		src.spatialBlend = 1f; // 완전한 3D 사운드
		src.minDistance = 1f;
		src.maxDistance = 30f;
		src.rolloffMode = AudioRolloffMode.Logarithmic;

		src.Play();
		Destroy(go, src.clip.length); // 소리 끝나면 삭제
	}

	public static SoundManager GetInstance()
	{
		return instance;
	}
	private void Init()
	{
		// 저장된 볼륨값 불러오기
		bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
		sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

		GameObject bgmObject = new GameObject("BGM");
		bgmObject.transform.parent = transform;
		bgmPlayer = bgmObject.AddComponent<AudioSource>();
		bgmPlayer.playOnAwake = false;
		bgmPlayer.loop = true;
		bgmPlayer.volume = bgmVolume;
		GameObject sfxObject = new GameObject("SFX");
		sfxObject.transform.parent = transform;
		sfxSound = new AudioSource[channels];

		for (int i = 0; i < sfxSound.Length; i++)
		{
			sfxSound[i] = sfxObject.AddComponent<AudioSource>();
			sfxSound[i].playOnAwake = false;
			sfxSound[i].volume = sfxVolume;
		}
	}
	public void SetSoundBgm(float volume)
	{
		bgmPlayer.volume = volume;
	}

	public void ReduceSoundBgm()
	{
		if (bgmPlayer.volume >= 0)
		{
			bgmPlayer.volume -= Time.deltaTime * reduceSoundSpeed;
		}
	}

	[Header("#BGM")]
	public AudioClip[] bgmClips;
	public float bgmVolume;
	AudioSource bgmPlayer;
	public float reduceSoundSpeed;


	[Header("#SFX")]
	public AudioClip[] sfxClips;
	public float sfxVolume;
	public int channels;
	AudioSource[] sfxSound;



	static SoundManager instance;


}