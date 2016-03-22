using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game {
	public class RoleData {
		/// <summary>
		/// 主键Id
		/// </summary>
		public string Id;
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name;
		/// <summary>
		/// 描述
		/// </summary>
		public string Desc;
		/// <summary>
		/// Icon Id
		/// </summary>
		public string IconId;
		/// <summary>
		/// 半身像Id
		/// </summary>
		public string HalfBodyId;
		/// <summary>
		/// 是否为主角
		/// </summary>
		public bool IsHost;
		/// <summary>
		/// 门派
		/// </summary>
		public OccupationType Occupation;
		/// <summary>
		/// 性别
		/// </summary>
		public GenderType Gender;
		/// <summary>
		/// 道德值
		/// </summary>
		public int Moral;
		int _hp;
		/// <summary>
		/// 气血
		/// </summary>
		public int HP {
			set {
				_hp = value;
			}
			get {
				_hp = _hp < 0 ? 0 : _hp;
				_hp = _hp > (MaxHP + MaxHPPlus) ? (MaxHP + MaxHPPlus) : _hp;
				return _hp;
			}
		}
		/// <summary>
		/// 最大气血
		/// </summary>
		public int MaxHP;
		/// <summary>
		/// 最大气血增量
		/// </summary>
		public int MaxHPPlus;
		/// <summary>
		/// 气血剩余比例
		/// </summary>
		public float HPRate {
			get {
				return (float)HP / (float)(MaxHP + MaxHPPlus);	
			}
		}
		/// <summary>
		/// 外功
		/// </summary>
		public float PhysicsAttack;
		/// <summary>
		/// 外功增量
		/// </summary>
		public float PhysicsAttackPlus;
		/// <summary>
		/// 外防
		/// </summary>
		public float PhysicsDefense;
		/// <summary>
		/// 外放增量
		/// </summary>
		public float PhysicsDefensePlus;
		/// <summary>
		/// 内功
		/// </summary>
		public float MagicAttack;
		/// <summary>
		/// 内功增量
		/// </summary>
		public float MagicAttackPlus;
		/// <summary>
		/// 内防
		/// </summary>
		public float MagicDefense;
		/// <summary>
		/// 内防增量
		/// </summary>
		public float MagicDefensePlus;
		/// <summary>
		/// 攻速增量
		/// </summary>
		public float AttackSpeedPlus;
		float _attackSpeed;
		/// <summary>
		/// 攻速[1-50]
		/// </summary>
		public float AttackSpeed {
			set {
				_attackSpeed = value;
			}
			get {
				return Mathf.Clamp(_attackSpeed + AttackSpeedPlus, 1, 50);
			}
		}
		/// <summary>
		/// 轻功[0-100]
		/// </summary>
		public float Dodge;
		/// <summary>
		/// 轻功增量
		/// </summary>
		public float DodgePlus;
		/// <summary>
		/// 引用的秘籍id集合
		/// </summary>
		public List<string> ResourceBookDataIds;
		/// <summary>
		/// 秘籍集合
		/// </summary>
		public List<BookData> Books;
		/// <summary>
		/// 引用的武器Id集合
		/// </summary>
		public string ResourceWeaponDataId;
		/// <summary>
		/// 当前兵器
		/// </summary>
		public WeaponData Weapon;
		/// <summary>
		/// 伤势类型
		/// </summary>
		public InjuryType Injury;
		/// <summary>
		/// 固定伤害值
		/// </summary>
		public int FixedDamage;
		/// <summary>
		/// 固定伤害值增量
		/// </summary>
		public int FixedDamagePlus;
		/// <summary>
		/// 伤害比例[1]
		/// </summary>
		public float DamageRate {
			get {
				return 1;
			}
		}
		/// <summary>
		/// 伤害比例增量[0-1]
		/// </summary>
		public float DamageRatePlus;
		/// <summary>
		/// 减伤比例[1]
		/// </summary>
		public float HurtCutRate {
			get {
				return 1;
			}
		}
		/// <summary>
		/// 减伤比例增量[0-1]
		/// </summary>
		public float HurtCutRatePlus;

		/// <summary>
		/// 可以释放技能
		/// </summary>
		public bool CanUseSkill;

		/// <summary>
		/// 可以切换门客
		/// </summary>
		public bool CanChangeRole;

		/// <summary>
		/// 不会误伤自己(50%概率误伤自己)
		/// </summary>
		public bool CanNotMakeMistake;

		/// <summary>
		/// 当前使用的秘籍索引
		/// </summary>
		int selectedBookIndex;

		/// <summary>
		/// 当前选中的书
		/// </summary>
		/// <value>The index of the selected book.</value>
		public int SelectedBookIndex {
			get {
				return selectedBookIndex;
			}
		}

		/// <summary>
		/// 角色死亡音效Id
		/// </summary>
		public string DeadSoundId;

		/// <summary>
		/// 出生的城镇Id
		/// </summary>
		public string HometownCityId;

		/// <summary>
		/// 角色状态
		/// </summary>
		public RoleStateType State;

		public RoleData() {
			ResourceBookDataIds = new List<string>();
			Books = new List<BookData>();
			ResourceWeaponDataId = "";
			IconId = "";
			HalfBodyId = "";
			Desc = "";
			selectedBookIndex = 0;
			Moral = 0;
			HP = 100;
			MaxHP = 100;
			Dodge = 10;
			PhysicsAttack = 10;
			PhysicsDefense = 0;
			MagicAttack = 10;
			MagicDefense = 0;
			FixedDamage = 0;
			DeadSoundId = "die0007";
			HometownCityId = "";
		}

		/// <summary>
		/// 初始化
		/// </summary>
		public void Init() {
			selectedBookIndex = 0;
			ClearPluses();
		}

		/// <summary>
		/// 计算对对方的外功伤害值
		/// </summary>
		/// <returns>The physics damage.</returns>
		/// <param name="toRole">To role.</param>
		public int GetPhysicsDamage(RoleData toRole) {
			float randomPhysicsAttack = Random.Range(0.95f, 1.05f) * (PhysicsAttack + PhysicsAttackPlus);
			return (int)((Mathf.Pow(randomPhysicsAttack, 2) / (randomPhysicsAttack + (toRole.PhysicsDefense + toRole.PhysicsDefensePlus)) + (FixedDamage + FixedDamagePlus)) * (DamageRate + DamageRatePlus) * (toRole.HurtCutRate - toRole.HurtCutRatePlus));
		}

		/// <summary>
		/// 计算对对方的内功伤害值
		/// </summary>
		/// <returns>The physics damage.</returns>
		/// <param name="toRole">To role.</param>
		public int GetMagicDamage(RoleData toRole) {
			float randomMagicAttack = Random.Range(0.95f, 1.05f) * (MagicAttack + MagicAttackPlus);
			return (int)((Mathf.Pow(MagicAttack, 2) / (MagicAttack + (toRole.MagicDefense + toRole.MagicDefensePlus)) + (FixedDamage + FixedDamagePlus)) * (DamageRate + DamageRatePlus) * (toRole.HurtCutRate - toRole.HurtCutRatePlus));
		}

		/// <summary>
		/// 判断对方的闪避概率
		/// </summary>
		/// <returns><c>true</c>, if will miss was checked, <c>false</c> otherwise.</returns>
		/// <param name="">.</param>
		public int GetMissRate(RoleData toRole) {
			float dodge = Mathf.Clamp(Dodge + DodgePlus, 0, 100);
			float toDodge = Mathf.Clamp(toRole.Dodge + toRole.DodgePlus, 0, 100);
			return (int)((Mathf.Pow(toDodge, 2) / (dodge + toDodge)) * 0.8f);
		}

		/// <summary>
		/// 判断对方是否闪避
		/// </summary>
		/// <returns><c>true</c> if this instance is hited the specified toRole; otherwise, <c>false</c>.</returns>
		/// <param name="toRole">To role.</param>
		public bool IsHited(RoleData toRole) {
			return Random.Range(1, 100) >= GetMissRate(toRole);
		}

		/// <summary>
		/// 获取当前使用秘籍
		/// </summary>
		/// <returns>The current book.</returns>
		public BookData GetCurrentBook() {
			if (Books == null || Books.Count == 0) {
				return null;
			}
			return Books[selectedBookIndex];
		}

		/// <summary>
		/// 切换秘籍
		/// </summary>
		/// <param name="index">Index.</param>
		public void SelectBook(int index) {
			if (Books == null || Books.Count == 0) {
				return;
			}
			selectedBookIndex = index < 0 ? 0 : index;
			selectedBookIndex %= Books.Count;
		}

		/// <summary>
		/// 处理气血值增减
		/// </summary>
		/// <param name="hurtHP">Hurt H.</param>
		public void DealHP(int hurtHP) {
			HP += hurtHP;
		}

		/// <summary>
		/// 清除增量
		/// </summary>
		public void ClearPluses() {
			MaxHPPlus = 0;
			AttackSpeedPlus = Weapon != null ? Weapon.AttackSpeedPlus : 0;
			DamageRatePlus = Weapon != null ? Weapon.DamageRatePlus : 0;
			FixedDamagePlus = Weapon != null ? Weapon.FixedDamagePlus : 0;
			DodgePlus = 0;
			HurtCutRatePlus = 0;
			MagicAttackPlus = 0;
			MagicDefensePlus = 0;
			PhysicsAttackPlus = Weapon != null ? Weapon.PhysicsAttackPlus : 0;
			PhysicsDefensePlus = 0;
			CanUseSkill = true;
			CanChangeRole = true;
			CanNotMakeMistake = true;
		}

		/// <summary>
		/// 将索引映射成实体类
		/// </summary>
		public void MakeJsonToModel() {
			Books.Clear();
			BookData book;
			for (int i = 0; i < ResourceBookDataIds.Count; i++) {
				book = JsonManager.GetInstance().GetMapping<BookData>("Books", ResourceBookDataIds[i]);
				book.MakeJsonToModel();
				Books.Add(book);
			}
			if (ResourceWeaponDataId != "") {
				Weapon = JsonManager.GetInstance().GetMapping<WeaponData>("Weapons", ResourceWeaponDataId);
			}
			else {
				Weapon = null;
			}
		}
	}
}
