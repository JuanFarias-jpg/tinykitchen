using Photon.Pun;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class PlayerHealthMulti : MonoBehaviourPun, IPunObservable
{
    [Header("Variables")]
    public IntVariable playerHP;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Header("Configuración")]
    public int damageAmount = 1;
    public float invincibilityTime = 1f;
    public int maxHP = 5;

    [Header("Parpadeo")]
    public float blinkInterval = 0.2f;

    [Header("Audios")]
    public AudioSource Damage;
    public AudioSource Dead;

    private Renderer[] renderers;
    private bool isInvincible = false;
    private bool isDeadLocal = false;

    public static bool IsDead = false;

    private void Awake()
    {
        IsDead = false;
        isDeadLocal = false;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        if (photonView.IsMine && playerHP != null)
            playerHP.Value = maxHP;
    }

    // ====================== DAÑO ======================
    public void TakeDamage()
    {
        TakeDamage(damageAmount);
    }

    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine || isInvincible || isDeadLocal) return;

        playerHP.Value -= damage;
        playerHP.Value = Mathf.Max(0, playerHP.Value);

        if (Damage != null) Damage.Play();

        if (playerHP.Value <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityFrames());

        // Sincronizar a los demás jugadores
        photonView.RPC("RPC_TakeDamage", RpcTarget.Others, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(int damage)
    {
        if (Damage != null) Damage.Play();
        StartCoroutine(InvincibilityFrames());
    }

    public void Heal(int amount)
    {
        if (!photonView.IsMine || isDeadLocal) return;

        playerHP.Value += amount;
        playerHP.Value = Mathf.Clamp(playerHP.Value, 0, maxHP);

        photonView.RPC("RPC_Heal", RpcTarget.Others, amount);
    }

    [PunRPC]
    private void RPC_Heal(int amount) { }

    private void Die()
    {
        if (isDeadLocal) return;

        isDeadLocal = true;
        IsDead = true;

        if (Dead != null) Dead.Play();

        DisablePlayerComponents();

        if (animator != null)
        {
            animator.CrossFade("muerteChef", 0f);
            animator.SetBool("IsDead", true);
        }

        if (GameManager.Instance != null)
            GameManager.Instance.LoseGame();

        photonView.RPC("RPC_Die", RpcTarget.Others);

        Destroy(gameObject, 3.5f);
    }

    [PunRPC]
    private void RPC_Die()
    {
        isDeadLocal = true;
        IsDead = true;
        DisablePlayerComponents();

        if (animator != null)
        {
            animator.CrossFade("muerteChef", 0f);
            animator.SetBool("IsDead", true);
        }
        if (Dead != null) Dead.Play();
    }

    private void DisablePlayerComponents()
    {
        if (TryGetComponent(out PlayerAnimator pa)) pa.enabled = false;
        if (TryGetComponent(out PlayerMovement mov)) mov.enabled = false;
        if (TryGetComponent(out PlayerCombat combat)) combat.enabled = false;
        if (TryGetComponent(out CharacterController cc)) cc.enabled = false;
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibilityTime)
        {
            SetRenderers(false);
            yield return new WaitForSeconds(blinkInterval);
            SetRenderers(true);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2f;
        }
        SetRenderers(true);
        isInvincible = false;
    }

    private void SetRenderers(bool state)
    {
        foreach (Renderer r in renderers)
            if (r != null) r.enabled = state;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerHP.Value);
            stream.SendNext(isDeadLocal);
        }
        else
        {
            playerHP.Value = (int)stream.ReceiveNext();
            isDeadLocal = (bool)stream.ReceiveNext();
            IsDead = isDeadLocal;
        }
    }
}