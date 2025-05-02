using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Entity : MonoBehaviour , IDamageable
{
    public virtual T Get<T>() where T : BuildableData
    {
        return this.dataDef as T;
    }
    public static GameObject MakeEntityFor(BuildableData build)
    {
        GameObject obj = new GameObject(build.name);
        Type type = GenTypes.GetTypeInAnyAssembly(build.entityClass);
        if(typeof(Entity).IsAssignableFrom(type) == false)
        {
            Debug.LogError("Entity class " + build.entityClass + " is not a subclass of Entity.");
            return null;
        }
        obj.AddComponent(type);
        Entity ent = obj.GetComponent<Entity>();
        ent.dataDef = build;

        if(build.comps != null && build.comps.Count > 0)
        {
            foreach (var item in build.comps)
            {
                Type compType = System.Type.GetType(item);
                if (compType != null)
                {
                    if (compType.IsSubclassOf(typeof(Component)) == false)
                    {
                        Debug.LogError("Component type " + item + " is not a subclass of Component.");
                        continue;
                    }
                    if (compType.IsAbstract)
                    {
                        Debug.LogError("Component type " + item + " is abstract.");
                        continue;
                    }
                    if (compType.IsGenericTypeDefinition)
                    {
                        Debug.LogError("Component type " + item + " is a generic type definition.");
                        continue;
                    }
                
                    obj.AddComponent(compType);
                }
                else
                {
                    Debug.LogError("Component type " + item + " not found for entity " + build.name);
                }
            }
        }
        

        ent.CallSpriteSetup();
        return obj;
    }

    public virtual void Awake(){}
    public virtual void Start()
    {
        SetDefault();
        if(sprites.Count > 0)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetSprite;
        }
        
    }
    public virtual void Update()
    {
        SpriteRender();
    }
    public virtual void FixedUpdate(){}
    public virtual void LateUpdate(){}
    public virtual void OnEnable(){}
    public virtual void OnDisable(){}
    public virtual void OnDestroy(){}
    public virtual void OnGUI(){}

    public virtual void OnTriggerEnter2D(Collider2D other){}
    public virtual void OnTriggerExit2D(Collider2D other){}
    public virtual void OnTriggerStay2D(Collider2D other){}
    public virtual void OnCollisionEnter2D(Collision2D other){}
    public virtual void OnCollisionExit2D(Collision2D other){}
    public virtual void OnCollisionStay2D(Collision2D other){}

    public virtual void DeSpawm(string mode = "")
    {
        GameObject.Destroy(this.gameObject);
    }

    public void CallSpriteSetup()
    {
        if(dataDef.graphicType != null)
        {
            Texture2D tex = TextureStorage.GetDatabase.GetTexture(dataDef.graphicType.path);

            if((dataDef.graphicType as GraphicSingleType) != null)
            {
                GraphicSingleType gr = (dataDef.graphicType as GraphicSingleType);
                Sprite sprite = Sprite.Create(tex, new Rect(gr.texture.x, gr.texture.y, gr.texture.w, gr.texture.h), gr.texture.pivot, gr.texture.pixelPerUnit);
                sprites.Add(sprite);
            }
            else if((dataDef.graphicType as GraphicMultiType) != null)
            {
                GraphicMultiType gr = (dataDef.graphicType as GraphicMultiType);
                for(int i = 0; i < gr.textures.Count; i++)
                {
                    Sprite sprite = Sprite.Create(tex, new Rect(gr.textures[i].x, gr.textures[i].y, gr.textures[i].w, gr.textures[i].h), gr.textures[i].pivot, gr.textures[i].pixelPerUnit);
                    sprites.Add(sprite);
                }
                isAnimation = gr.isAnimation;
            }
        }
    }
    
    public virtual Sprite GetSprite
    {
        get
        {
            if(!isAnimation)
            {
                if(sprites.Count > 0)
                {
                    return sprites[0];
                }
                else
                {
                    Debug.LogError("No sprite found for " + dataDef.name);
                    return null;
                }
            }
            else
            {
                return sprites[currentFrame];
                //Debug.LogError("Entity is animated, use GetAnimation() instead of GetSprite()");
            }
        }
    }

    public virtual void ExecuteAction(string actionName)
    {
        IAction action = GetComponent(actionName) as IAction;
        if(action != null)
        {
            action.Execute(this.gameObject);
        }
        else
        {
            Debug.LogError("Action " + actionName + " not found on entity " + gameObject.name);
        }
    }

    public virtual void SpriteRender()
    {
        if(isAnimation)
        {
            timer += Time.deltaTime;
            if(timer >= (1f / (dataDef.graphicType as GraphicMultiType).textures[currentFrame].frameRate))
            {
                currentFrame++;
                if(currentFrame >= (dataDef.graphicType as GraphicMultiType).textures.Count)
                {
                    currentFrame = 0;
                }
                timer = 0;
            }
            spriteRenderer.sprite = GetSprite;
        }
    }
    public virtual void SetDefault()
    {
    }

    public void TakeDamage(Entity attacker)
    {
        if(hitPoint > 0)
        {
            hitPoint -= attacker.DoDameToEntity();
        }

        if(hitPoint <= 0)
        {
            Debug.Log("Entity was destroy by " + attacker.name);
            GameObject.Destroy(this.gameObject);
        }
    }

    public virtual float DoDameToEntity()
    {
        return 1;
    }

    public bool showHint = false;
    private bool isAnimation = false;
    private float timer = 0f;
    private int currentFrame = 0;
    public List<Sprite> sprites = new List<Sprite>();
    public SpriteRenderer spriteRenderer;
    

    [SerializeField]
    public BuildableData dataDef;
    public float hitPoint = 10;
}
