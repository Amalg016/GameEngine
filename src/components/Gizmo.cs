using GameEngine.Core.Platform;
using GameEngine.ECS;
using GameEngine.editor;
using GameEngine.scenes;
using System.Numerics;

namespace GameEngine.components
{
    public class Gizmo : Component
    {
        private Vector4 xAxisColor = new Vector4(1, 0.3f, 0.3f, 1);
        private Vector4 xAxisColorHover = new Vector4(1, 0, 0, 1);
        private Vector4 yAxisColor = new Vector4(0.3f, 1, 0.3f, 1);
        private Vector4 yAxisColorHover = new Vector4(0, 1, 0, 1);
        private Vector2 position;

        private Vector2 xAxisOffset = new Vector2(.3f, -6f / 80);
        private Vector2 yAxisOffset = new Vector2(-0f, .25f);

        private GameObject xAxixObject;
        private GameObject yAxisObject;
        private SpriteRenderer xAxisSprite;
        private SpriteRenderer yAxisSprite;
        protected GameObject activeGameObject = null;
        private PropertiesWindow propertiesWindow;

        private float gizmoWidth = 16 / 80f;
        private float gizmoHeight = 48 / 80f;

        protected bool xAxisActive = false;
        protected bool yAxisActive = false;

        private bool Using = false;
        public Gizmo(Sprite sprite, PropertiesWindow properties)
        {
            this.xAxixObject = Prefab.generateSpriteObject(sprite, gizmoWidth, gizmoHeight, 100);
            this.yAxisObject = Prefab.generateSpriteObject(sprite, gizmoWidth, gizmoHeight, 100);
            this.xAxisSprite = this.xAxixObject.GetComponent<SpriteRenderer>();
            //  Console.WriteLine(xAxisSprite.sprite.SpritesheetName);
            this.yAxisSprite = this.yAxisObject.GetComponent<SpriteRenderer>();
            this.xAxixObject.AddComponent(new NonPickable());
            this.yAxisObject.AddComponent(new NonPickable());
            propertiesWindow = properties;

            SceneManager.CurrentScene.addGameObjectToScene(this.xAxixObject);
            SceneManager.CurrentScene.addGameObjectToScene(this.yAxisObject);
        }
        public override void Load()
        {
            xAxixObject.transform.rotation = 90;
            yAxisObject.transform.rotation = 180;
            this.xAxixObject.dontSerialize();
            this.yAxisObject.dontSerialize();
        }

        public override void Update()
        {
            if (!Using)
            {
                this.setInactive();
            }
        }

        public override void EditorUpdate()
        {
            if (!Using)
            {
                return;
            }

            this.activeGameObject = PropertiesWindow.getActiveGameObject();

            //Refactor later
            if (activeGameObject != null)
            {
                if (InputManager.duplPressed)
                {
                    GameObject newObj = this.activeGameObject.Copy();
                    SceneManager.CurrentScene.addGameObjectToScene(newObj);
                    newObj.transform.position += new Vector2(0.1f, 0.1f);
                    PropertiesWindow.setActiveGameObject(newObj);
                    return;
                }
                else if (InputManager.delete)
                {
                    activeGameObject.Destroy();
                    this.setInactive();
                    PropertiesWindow.setActiveGameObject(null);
                }
            }

            if (this.activeGameObject != null)
            {
                this.setActive();
            }
            else
            {
                this.setInactive();
                return;
            }

            bool xAxisHot = CheckXhoverState();
            bool yAxisHot = CheckYhoverState();

            if ((xAxisHot || xAxisActive) && InputManager.isDragging)
            {
                xAxisActive = true;
                yAxisActive = false;
            }
            else if ((yAxisHot || yAxisActive) && InputManager.isDragging)
            {
                xAxisActive = false;
                yAxisActive = true;
            }
            else if (!InputManager.isDragging)
            {
                xAxisActive = false;
                yAxisActive = false;
            }

            if (activeGameObject != null)
            {
                this.yAxisObject.transform.position.X = this.activeGameObject.transform.position.X;
                this.yAxisObject.transform.position.Y = this.activeGameObject.transform.position.Y;
                yAxisObject.transform.position += yAxisOffset;
                this.xAxixObject.transform.position.X = this.activeGameObject.transform.position.X;
                this.xAxixObject.transform.position.Y = this.activeGameObject.transform.position.Y;
                xAxixObject.transform.position += xAxisOffset;
            }


        }

        private bool CheckYhoverState()
        {
            Vector2 mousePos = new Vector2(InputManager.GetOrthoX(), InputManager.GetOrthoY());
            if (mousePos.X <= yAxisObject.transform.position.X + (gizmoWidth / 2f) && mousePos.X >= yAxisObject.transform.position.X - (gizmoWidth / 2f) && mousePos.Y <= yAxisObject.transform.position.Y + (gizmoHeight / 2f) && mousePos.Y >= yAxisObject.transform.position.Y - (gizmoHeight / 2f))
            {
                yAxisSprite.setColor(yAxisColorHover);
                return true;
            }
            yAxisSprite.setColor(yAxisColor);
            return false;
        }

        private bool CheckXhoverState()
        {

            Vector2 mousePos = new Vector2(InputManager.GetOrthoX(), InputManager.GetOrthoY());
            if (mousePos.X <= xAxixObject.transform.position.X + (gizmoHeight / 2f) && mousePos.X >= xAxixObject.transform.position.X - (gizmoWidth / 2.0f) && mousePos.Y >= xAxixObject.transform.position.Y - (gizmoHeight / 2f) && mousePos.Y <= xAxixObject.transform.position.Y + (gizmoWidth / 2f))
            {
                xAxisSprite.setColor(xAxisColorHover);
                return true;
            }
            xAxisSprite.setColor(xAxisColor);
            return false;
        }

        public void setActive()
        {
            xAxisSprite.setColor(xAxisColor);
            yAxisSprite.setColor(yAxisColor);
        }

        private void setInactive()
        {
            activeGameObject = null;
            xAxisSprite.setColor(new Vector4(0, 0, 0, 0));
            yAxisSprite.setColor(new Vector4(0, 0, 0, 0));
        }
        public void setUsing()
        {
            Using = true;
        }
        public void setNotUsing()
        {
            Using = false;
            this.setInactive();
        }
    }
}
