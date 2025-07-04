using UnityEngine;

public class Anim : MonoBehaviour
{
    public Animator anim;

    [Header("Sound Effects")]
    [SerializeField] AudioSource SFXAudioSource;
    [Header("Dash")]
    [SerializeField] AudioClip dashSound;
    [Header("AirBlast")]
    [SerializeField] AudioClip airBlastSound;
    [Header("WallRun")]
    [SerializeField] AudioClip wallRunSound;
    [Header("Footstep Sounds")]
    [SerializeField] AudioClip[] footstepSoundsWood;
    [SerializeField] AudioClip[] footstepSoundsGrass;
    [SerializeField] AudioClip[] footstepSoundsStone;
    [SerializeField] AudioSource footstepAudioSource;
    [Header("References")]
    [SerializeField] public Rigidbody rb;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private WallRun wallRun;
    [SerializeField] private Shotgun shotgun;
    [SerializeField] private GameObject AirBlastExplosionEffect;
    [SerializeField] private GameObject AirBlastEffect;

    private int lastFootstepIndexWood = -1;
    private int lastFootstepIndexGrass = -1;
    private int lastFootstepIndexStone = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float velocityMagnitude = rb.linearVelocity.magnitude;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            anim.SetFloat("Speed",Mathf.Log(velocityMagnitude + 1, 5f)); // Adjust speed based on velocity
        }

        if (playerMovement.State == PlayerMovement.MovementState.Running)
        {
            anim.SetBool("IsGround", true);
        }
        else if (playerMovement.State == PlayerMovement.MovementState.WallRunning)
        {
            if (wallRun.isWallLeft)
            {
                if (wallRunSound != null)
                {
                    if (SFXAudioSource != null)
                    {
                        // Ensure the correct clip is assigned
                        if (SFXAudioSource.clip != wallRunSound)
                        {
                            SFXAudioSource.clip = wallRunSound;
                        }
                        // Ensure the sound will loop
                        SFXAudioSource.loop = true;
                        // Play the sound if it's not already playing
                        if (!SFXAudioSource.isPlaying)
                        {
                            SFXAudioSource.Play();
                        }
                    }
                }
                anim.SetBool("IsWallLeft", true);
                anim.SetBool("IsWallRight", false);
            }
            else if (wallRun.isWallRight)
            {
                if (wallRunSound != null)
                {
                    if (SFXAudioSource != null)
                    {
                        // Ensure the correct clip is assigned
                        if (SFXAudioSource.clip != wallRunSound)
                        {
                            SFXAudioSource.clip = wallRunSound;
                        }
                        // Ensure the sound will loop
                        SFXAudioSource.loop = true;
                        // Play the sound if it's not already playing
                        if (!SFXAudioSource.isPlaying)
                        {
                            SFXAudioSource.Play();
                        }
                    }
                }
                anim.SetBool("IsWallRight", true);
                anim.SetBool("IsWallLeft", false);
            }
        }
        else
        {
            if (SFXAudioSource != null)
            {
                SFXAudioSource.loop = false;
                SFXAudioSource.Stop();
            }
            anim.SetBool("IsWallRight", false);
            anim.SetBool("IsWallLeft", false);
            anim.SetBool("IsGround", false);
        }
        if (shotgun != null)
        {
            if (shotgun.IsDash)
            {
                anim.SetBool("IsDash", true);
                if (dashSound != null)
                {
                    SFXAudioSource.PlayOneShot(dashSound);
                    SFXAudioSource.loop = false;
                }
                anim.Play("DashForward");
                disableDash();
            }
            if (shotgun.IsAirBlast)
            {
                print("AirBlast");
                anim.SetBool("IsAirBlast", true);
                if (airBlastSound != null)
                {
                    AirBlastExplosionEffect.SetActive(true);
                    SFXAudioSource.PlayOneShot(airBlastSound);
                    SFXAudioSource.loop = false;
                }
                anim.Play("AirBlast");
                disableAirBlast();
            }
        }
    }
    void disableAirBlast()
    {
        if (AirBlastExplosionEffect != null)
        {
            AirBlastEffect.SetActive(false);
            AirBlastExplosionEffect.SetActive(false);
        }
        shotgun.IsAirBlast = false;
        anim.SetBool("IsAirBlast", false);
    }
    void disableDash()
    {
        if (AirBlastExplosionEffect != null)
        {
            AirBlastEffect.SetActive(false);
            AirBlastExplosionEffect.SetActive(false);
        }
        shotgun.IsDash = false;
        anim.SetBool("IsDash", false);
    }
    void EnableExplosionEffect()
    {
        if (AirBlastExplosionEffect != null)
        {
            AirBlastExplosionEffect.SetActive(true);
        }
    }
    void EnableAirBlastEffect()
    {
        if (AirBlastEffect != null)
        {
            AirBlastEffect.SetActive(true);
        }
    }
    void playfootstep()
    {
        if (footstepAudioSource != null)
        {
            AudioClip clip = null;
            int newIndex = -1;

            if (playerMovement != null && playerMovement.State == PlayerMovement.MovementState.WallRunning)
            {
                // Special handling for WallRunning, always use stone sounds
                if (footstepSoundsStone != null && footstepSoundsStone.Length > 0)
                {
                    do
                    {
                        newIndex = Random.Range(0, footstepSoundsStone.Length);
                    } while (newIndex == lastFootstepIndexStone && footstepSoundsStone.Length > 1);
                    clip = footstepSoundsStone[newIndex];
                    lastFootstepIndexStone = newIndex;
                }
            }
            else
            {
                // Handle footstep sounds based on surface type
                switch (playerMovement.currentSurface)
                {
                    case PlayerMovement.SurfaceType.Wood:
                        if (footstepSoundsWood != null && footstepSoundsWood.Length > 0)
                        {
                            do
                            {
                                newIndex = Random.Range(0, footstepSoundsWood.Length);
                            } while (newIndex == lastFootstepIndexWood && footstepSoundsWood.Length > 1);
                            clip = footstepSoundsWood[newIndex];
                            lastFootstepIndexWood = newIndex;
                        }
                        break;
                    case PlayerMovement.SurfaceType.Grass:
                        if (footstepSoundsGrass != null && footstepSoundsGrass.Length > 0)
                        {
                            do
                            {
                                newIndex = Random.Range(0, footstepSoundsGrass.Length);
                            } while (newIndex == lastFootstepIndexGrass && footstepSoundsGrass.Length > 1);
                            clip = footstepSoundsGrass[newIndex];
                            lastFootstepIndexGrass = newIndex;
                        }
                        break;
                    case PlayerMovement.SurfaceType.Stone:
                        if (footstepSoundsStone != null && footstepSoundsStone.Length > 0)
                        {
                            do
                            {
                                newIndex = Random.Range(0, footstepSoundsStone.Length);
                            } while (newIndex == lastFootstepIndexStone && footstepSoundsStone.Length > 1);
                            clip = footstepSoundsStone[newIndex];
                            lastFootstepIndexStone = newIndex;
                        }
                        break;
                }
            }

            if (clip != null)
            {
                footstepAudioSource.PlayOneShot(clip);
            }
        }
    }
}
