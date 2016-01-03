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
		/// 气血
		/// </summary>
		public int HP;
		/// <summary>
		/// 最大气血
		/// </summary>
		public int MaxHP;
		/// <summary>
		/// 气血剩余比例
		/// </summary>
		public float HPRate {
			get {
				return (float)HP / (float)MaxHP;	
			}
		}
		/// <summary>
		/// 外功
		/// </summary>
		public float PhysicsAttack;
		/// <summary>
		/// 外防
		/// </summary>
		public float PhysicsDefense;
		/// <summary>
		/// 内功
		/// </summary>
		public float MagicAttack;
		/// <summary>
		/// 内防
		/// </summary>
		public float MagicDefense;
		/// <summary>
		/// 攻速
		/// </summary>
		public float AttackSpeed;
		/// <summary>
		/// 轻功
		/// </summary>
		public float Dodge;
		/// <summary>
		/// 秘籍集合
		/// </summary>
		public List<BookData> Books;
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
		/// 伤害比例
		/// </summary>
		public float DamageRate;
		/// <summary>
		/// 减伤比例
		/// </summary>
		public float HurtCutRate;
		/// <summary>
		/// 当前使用的秘籍索引
		/// </summary>
		int selectedBookIndex;

		List<BuffData> buffList;

		public RoleData() {
			Books = new List<BookData>();
			buffList = new List<BuffData>();
			selectedBookIndex = 0;
			Dodge = 10;
			PhysicsAttack = 100;
			MagicAttack = 100;
			HurtCutRate = 1;
			DamageRate = 1;
		}

		/// <summary>
		/// 计算对对方的外功伤害值
		/// </summary>
		/// <returns>The physics damage.</returns>
		/// <param name="toRole">To role.</param>
		public int GetPhysicsDamage(RoleData toRole) {
			return (int)((Mathf.Pow(PhysicsAttack, 2) / (PhysicsAttack + toRole.PhysicsDefense) + FixedDamage) * DamageRate * toRole.HurtCutRate);
		}

		/// <summary>
		/// 计算对对方的内功伤害值
		/// </summary>
		/// <returns>The physics damage.</returns>
		/// <param name="toRole">To role.</param>
		public int GetMagicDamage(RoleData toRole) {
			return (int)((Mathf.Pow(MagicAttack, 2) / (MagicAttack + toRole.MagicDefense) + FixedDamage) * DamageRate * toRole.HurtCutRate);
		}

		/// <summary>
		/// 判断对方的闪避概率
		/// </summary>
		/// <returns><c>true</c>, if will miss was checked, <c>false</c> otherwise.</returns>
		/// <param name="">.</param>
		public int GetMissRate(RoleData toRole) {
			float dodge = Mathf.Clamp(Dodge, 0, 100);
			float toDodge = Mathf.Clamp(toRole.Dodge, 0, 100);
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
			HP = HP < 0 ? 0 : HP;
			HP = HP > MaxHP ? MaxHP : HP;
		}
	}
}
