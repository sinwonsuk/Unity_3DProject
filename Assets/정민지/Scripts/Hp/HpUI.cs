using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviour
{
    [SerializeField] private int hp;
    [SerializeField] private int maxHp;
    [SerializeField] private Image hpFill;
    [SerializeField] private TMP_Text hpText;

    private void OnEnable()
    {
        EventBus<Hp>.OnEvent += GetHp;
    }

    private void OnDisable()
    {
        EventBus<Hp>.OnEvent -= GetHp;
    }

    private void Start()
    {
        hpFill.fillAmount = (float)hp / maxHp;
        hpText.text = ($"{hp} / {maxHp}");
        EventBus<Hp>.Raise(new Hp(hp));
    }

    private void GetHp(Hp _hp)
    {
        hp = _hp.hp;
        hpFill.fillAmount = (float)hp / maxHp;
        hpText.text = ($"{hp} / {maxHp}");
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        EventBus<Hp>.Raise(new Hp(hp));
    }

    public void UseHealingItem(int heal)
    {
        hp += heal;
        EventBus<Hp>.Raise(new Hp(hp));
    }
}
