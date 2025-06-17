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
        EventBus<Hp>.OnEvent += UpdateHp;
        EventBus<Damage>.OnEvent += TakeDamage;
        EventBus<Heal>.OnEvent += UseHealingItem;
    }

    private void OnDisable()
    {
        EventBus<Hp>.OnEvent -= UpdateHp;
        EventBus<Damage>.OnEvent -= TakeDamage;
        EventBus<Heal>.OnEvent -= UseHealingItem;
    }

    private void Start()
    {
        EventBus<Hp>.Raise(new Hp(hp));
    }

    private void UpdateHp(Hp _hp)
    {
        hp = _hp.currentHp;
        hpFill.fillAmount = (float)hp / maxHp;
        hpText.text = ($"{hp} / {maxHp}");
    }

    public void TakeDamage(Damage damage)
    {
        hp -= damage.damage;
        EventBus<Hp>.Raise(new Hp(hp));
    }

    public void UseHealingItem(Heal heal)
    {
        hp += heal.heal;
        EventBus<Hp>.Raise(new Hp(hp));
    }
}
