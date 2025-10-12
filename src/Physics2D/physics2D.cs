using GameEngine.Physics2D.components;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Transform = GameEngine.components.Transform;
using GameEngine.Core.Math;

namespace GameEngine.Physics2D
{
    public class physics2D
    {
        Vector2 gravity = new Vector2(0, -9.8f);
        World world = new World();
        private float physicsTime = 0;
        float physicsTimeStep = 1 / 60;
        int positionIterations = 3;
        int velocityIterations = 10;
        SolverIterations solver;


        public physics2D()
        {
            solver = new SolverIterations();
            solver.VelocityIterations = velocityIterations;
            solver.PositionIterations = positionIterations;
        }
        public void Add(GameObject go)
        {
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            if (rb != null && rb.rawBody == null)
            {
                Transform transform = go.transform;

                //          BodyDef bodyDef = new BodyDef();
                //          bodyDef.Enabled = true; 
                //          bodyDef.Angle = (float)Mathf.DegreesToRadians(transform.rotation);
                //          bodyDef.Position=  transform.position;
                //          bodyDef.AngularDamping = rb.angularDamping;
                //          bodyDef.LinearDamping=rb.linearDamping;
                //          bodyDef.FixedRotation = rb.fixedRotation;
                //          bodyDef.Bullet = rb.continuousCollision;
                //       //   bodyDef.GravityScale = 8;
                //          // bodyDef.Enabled = true;
                //          //bodyDef.AllowSleep = false;
                //         bodyDef.BodyType= rb.bodyType;
                //          // ShapeDef s = new ShapeDef();
                //        //     PolygonShape polygon=new PolygonShape();
                //        // CircleShape circle=new CircleShape();   
                CircleCollider circleCollider;
                Box2DCollider boxCollider;


                if ((circleCollider = go.GetComponent<CircleCollider>()) != null)
                {
                    // circle.Radius = circleCollider.radius;
                    Body body = this.world.CreateCircle(circleCollider.radius, rb.mass, new Vector2(transform.position.X + circleCollider.Offset.X, transform.position.Y + circleCollider.Offset.Y), rb.bodyType);
                    body.Rotation = transform.rotation;
                    rb.rawBody = body;
                    //       body. CreateFixture(circle, rb.mass);
                }
                else if ((boxCollider = go.GetComponent<Box2DCollider>()) != null)
                {
                    Vector2 halfSize = new Vector2(boxCollider.halfSize.X * 0.5f, boxCollider.halfSize.Y * 0.5f);
                    // Vector2 offset = boxCollider.Offset;
                    // Vector2 orgin = boxCollider.Orgin;
                    Body body = world.CreateRectangle(boxCollider.halfSize.X, boxCollider.halfSize.Y, rb.mass, new Vector2(transform.position.X + boxCollider.Orgin.X + boxCollider.Offset.X, boxCollider.Orgin.Y + boxCollider.Offset.Y + transform.position.Y), Mathf.DegreesToRadians(transform.rotation), rb.bodyType);
                    // polygon.SetAsBox(halfSize.X, halfSize.Y,orgin, transform.rotation);
                    //   Vector2 pos = bodyDef.Position;
                    //    float xPos = pos.X + offset.X;
                    //   float yPos = pos.Y + offset.Y;
                    //   bodyDef.Position.X=xPos;
                    //   bodyDef.Position.Y=yPos;
                    // bodyDef.Position += offset;
                    // Body body = this.world.CreateBody(bodyDef);
                    rb.rawBody = body;
                    // body.CreateFixture(polygon,rb.mass);
                    // body.SetLinearVelocity(new Vector2(10, 0));
                    // body.
                }

            }
        }

        public void Update()
        {
            physicsTime += Time.deltaTime;
            if (physicsTime >= 0)
            {
                physicsTime -= physicsTimeStep;
                world.Step(Time.deltaTime, ref solver);
            }
        }

        public virtual void Dispose()
        {
            world?.Clear();
            world = null;
        }

        public void DestroyGameObject(GameObject go)
        {
            Rigidbody2D rb = new Rigidbody2D();
            if (rb != null)
            {
                if (rb.rawBody != null)
                {
                    world.Remove(rb.rawBody);
                    rb.rawBody = null;
                }
            }
        }

    }
}
