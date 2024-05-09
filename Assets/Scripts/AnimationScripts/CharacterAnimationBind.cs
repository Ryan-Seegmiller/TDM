using UnityEngine;
public class CharacterAnimationBind : MonoBehaviour
{
    [HideInInspector]public Animator anim;
    Character character;
    private void Awake()
    {
        character = GetComponentInParent<Character>();
        anim=GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if (character != null)
        {
            SetAnimParams();

            if (!character.isDead) {
                DoStepAngles();
                if (character.characterType != CharacterType.blob && anim.GetLayerWeight(1) != 1)
                {
                    anim.SetLayerWeight(1, 1);//ensure arms layer is active whilst alive
                }
            }
            else if(character.characterType!=CharacterType.blob&& anim.GetLayerWeight(1)!=0)
            {
                anim.SetLayerWeight(1, 0);//turn of arms layer when dead
            }
        }
    }
    void SetAnimParams()
    {
        //anim params
        anim.SetInteger("z_Direction", character.movement.input_direction != Vector2.zero ? 1 : 0);
        anim.SetBool("attack", (bool)character.animationAttack);
        anim.SetBool("dead", (bool)character.isDead);
    }
    void DoStepAngles()
    {
        //step angles calculation
        if (character.movement.input_direction != Vector2.zero)
        {
            Vector3 angleVector = character.movement.input_direction;
            angleVector.x *= -1;
            float angle = Vector2.SignedAngle(new Vector2(0, 1), angleVector);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, angle, transform.localEulerAngles.z);
        }
    }

    void AnimationEnd()
    {
        character.StopAttackAnimation();
    }
    void PlayFootstepParticles()
    {  
        ParticleManager.PlayParticle(1, character.transform.position);
    }
}
