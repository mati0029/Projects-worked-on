﻿using UnityEngine;
using System.Collections;

public class Punch : MonoBehaviour, IHitBoxListener
{
	public bool m_CanHitMultipleEnemies = false;

	public float m_HorizontalKnockback;
	public float m_VerticalKnockback;

	public float m_PunchTime;
	public float m_TimeToCombo;

	float m_PunchTimer;
	float m_ComboTimer;

	HitBox m_HitBox;
	ParticleSystem m_HitParticles;

	int m_Combo;

	Stamina m_Stamina;

	void Start()
	{
		m_HitBox = GetComponentInChildren<HitBox> ();
		m_HitBox.RegisterListener(this);
		m_HitBox.gameObject.SetActive(false);
		
		m_HitParticles = GetComponentInChildren<ParticleSystem> ();

		m_Stamina = GetComponent<Stamina> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if(m_PunchTimer > 0.0f)
		{
			m_PunchTimer -= Time.deltaTime;

			if(m_PunchTimer <= 0.0f)
			{
				m_HitBox.gameObject.SetActive(false);

				m_ComboTimer = m_TimeToCombo;
			}
		}
		else if(InputManager.getPunchDown() && m_Stamina.stamina >= Constants.PUNCH_COST)
		{
			m_PunchTimer = m_PunchTime;

			m_HitBox.gameObject.SetActive(true);

			m_Stamina.stamina -= Constants.PUNCH_COST;
			
			// TODO: Play proper animation for combo

			if(m_ComboTimer > 0.0f)
			{
				m_Combo++;
				m_Combo &= 3;
			}
		}

		m_ComboTimer -= Time.deltaTime;

		if(m_ComboTimer <= 0.0f)
		{
			m_Combo = 0;
		}
	}

	public void OnHitBoxEnter(Collider otherCollider)
	{
		Police police = otherCollider.GetComponent<Police> ();
		
		if(police)
		{
			bool hasActuallyHit = police.Knockback(transform.forward * m_HorizontalKnockback + transform.up * m_VerticalKnockback);
			
			Vector3 newPosition = m_HitParticles.transform.position;
			newPosition.z = -0.5f;
			m_HitParticles.transform.position = newPosition;
			
			m_HitParticles.Play ();

			if(hasActuallyHit && !m_CanHitMultipleEnemies)
			{
				m_HitBox.gameObject.SetActive(false);
			}
		}
	}
}