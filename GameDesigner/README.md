## GDNet
 (Game Designer Network)专为游戏而设计的网络框架和动作状态机，使用C#开发，支持.NetFramework和Core版本，目前主要用于Unity3D，Form窗体程序和控制台项目开发。扩展性强，支持新协议快速扩展，当前支持tcp，gcp, udx, kcp, web网络协议。简易上手. api注释完整。

## 模块图

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/gdnet.png" width = "620" height = "700" alt="图片名称" align=center />

## 使用

<br>1.下载GameDesigner, 解压GameDesigner.zip, 打开Unity菜单Window/PackageManager管理器，点击+号的第一项add package on disk</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/packagemanager01.png" width = "490" height = "160" alt="图片名称" align=center />

<br>2.选择解压的路径xx/GameDesigner/GameDesigner/package.json即可导入gdnet包</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/packagemanager02.png" width = "960" height = "540" alt="图片名称" align=center />

<br>3.如果前面没有问题，最终显示的包界面</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/packagemanager03.png" width = "942" height = "575" alt="图片名称" align=center />

<br>4.打开BuildSettings->ProjectSettings->OtherSettings->设置 ApiCompatibilityLevel* = .NET 4.x 和 AllowUnsafeCode勾上，2021版本后是ApiCompatibilityLevel* = .NET Framework</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/gdnetsetting.png" width = "645" height = "239" alt="图片名称" align=center />

<br>5.创建服务器项目,使用控制台或窗体程序都可以，也可以统一在unity的Assembly-CSharp项目里添加新建服务器项目</br>
<br>在unity随便创建个脚本，双击进入VS代码编辑器， 然后右键解决方案，必须右键解决方案，必须右键解决方案，必须右键解决方案 重要的问题说三遍</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/step1.png" width = "672" height = "398" alt="图片名称" align=center />

<br>6.选择添加服务器项目，使用控制台项目</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/step2.png" width = "1024" height = "680" alt="图片名称" align=center />

<br>7.定义服务器名称，并且选择项目路径到你的unity项目根目录，和Assets同级的文件夹目录</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/step3.png" width = "1024" height = "680" alt="图片名称" align=center />

<br>8.右键解决方案，必须右键解决方案，必须右键解决方案，必须右键解决方案 重要的问题说三遍， 添加现有方案，现有方案，现有方案</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/step4.png" width = "560" height = "370" alt="图片名称" align=center />

<br>9.选择解压的GameDesigner目录，里面有GameDesigner.csproj文件</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/step5.png" width = "960" height = "540" alt="图片名称" align=center />

<br>10.右键Server的引用，弹出选项，选择添加引用</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/step6.png" width = "362" height = "205" alt="图片名称" align=center />

<br>11.选择项目选项，选择GameDesigner，确定即可</br>
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/step7.png" width = "800" height = "550" alt="图片名称" align=center />

<br>12.新建一个Service脚本文件, 这个就是你的服务器类</br>
```
internal class Client : NetPlayer//你的客户端类
{
}
internal class Scene : NetScene<Client>//你的游戏场景类
{
}
class Service : TcpServer<Client, Scene>//你的服务器类
{
    protected override bool OnUnClientRequest(Client unClient, RPCModel model)
    {
        Console.WriteLine(model.pars[0]);
        //你也可以理解为返回true则是输入的账号密码正确, 返回false则是账号或密码错误
        return true;//100%必须理解这个, 返回false则一直在这里被调用，无法调用带有[Rpc]特性的方法。 返回true后下次客户端SendRT("方法名"，参数)才能调用下面那些[Rpc]特性的方法
    }
    [Rpc(cmd = NetCmd.SafeCall)]//使用SafeCall指令后, 第一个参数插入客户端对象, 这个客户端对象就是哪个客户端发送,这个参数就是对应那个客户端的对象
    void test(Client client, string str) 
    {
        Console.WriteLine(str);
        SendRT(client, "test", "服务器rpc回调");//服务器回调
    }
}
```
<br>13.main入口方法写上</br>

```
var server = new Service();//创建服务器对象
server.Log += Console.WriteLine;//打印服务器内部信息
server.Run(6666);//启动6666端口
while (true)
{
    Console.ReadLine();
}
```

<br>14.创建客户端控制台项目， 跟服务器项目创建一样，看上面创建服务器项目教程</br>

<br>15.定义一个Test类, 用来测试rpc过程调用</br>

```
class Test 
{
    [Rpc]
    void test(string str) 
    {
        Console.WriteLine(str);
    }
}
```
<br>16.在main入口方法写上，这是控制台项目， 不要把这段代码用在unity，会死循环，卡死unity</br>
```
TcpClient client = new TcpClient();
client.Log += Console.WriteLine;
Test test = new Test();
client.AddRpcHandle(test);
client.Connect("127.0.0.1", 6666).Wait();
client.SendRT("test", "第一次进入服务器的OnUnClientRequest方法");
client.SendRT("test", "客户端rpc请求");
while (true)
{
    Console.ReadLine();
}
```
到此基本使用完成

## 网关转发Nginx

如果需要网关服务器的, 可使用Nginx代理转发到真实服务器, 加了Nginx代理后,服务器和客户端流程为 客户端:client->Nginx->gameServer, 服务器:gameServer->Nginx->client


```
worker_processes  1;
#error_log  logs/error.log;
#error_log  logs/error.log  notice;
#error_log  logs/error.log  info;
#pid        logs/nginx.pid;
events {
    #客户端连接数量
    worker_connections  10240;
}
stream{
    upstream gameServer{
        #这里是真实的游戏服务器,如果在阿里云,腾讯云,填写对应的ip即可
        server 127.0.0.1:6667;
    }
    server{
        #nginx的监听端口,客户端连接的端口
        listen 6666;
        proxy_pass gameServer;
    }
}
```
详情请看:https://www.cnblogs.com/knowledgesea/p/6497783.html

## MySql使用

1.mysql安装可到gdnet群:825240544群文件去下载, 或者官网下载:http://c.biancheng.net/view/2391.html
2.mysql可视化界面工具Navicat, 到这里下载:https://www.cnblogs.com/yx-man/p/13220878.html
3.以上两步好后, 打开Navicat创建mysql数据库, 创建表和字段后, 即可用mysql orm工具生成*.cs类文件:https://gitee.com/leng_yue/my-sql-data-build
4.orm工具生成的文件可拖入unity项目, 使用unity菜单GameDesigner/Network/ExternalReference工具添加文件引用

## 对象池
gdnet提供BufferPool二进制数据对象池和ObjectPool类对象池, 在网络代码内部采用了BufferPool对象池, 使得网络可以高速读写处理数据, 而不是每次要创建一个byte[]来处理!


```
var seg = BufferPool.Take(65535);//申请65535字节的内存片
seg.WriteValue(123);//写入4字节的值
BufferPool.Push(seg);//压入内存片,等待下次复用
var seg1 = BufferPool.Take(65535);//这次的申请内存片,实际是从BufferPool中弹出seg对象,在这个过程中不会再创建byte[65535]
seg1.WriteValue(456);
BufferPool.Push(seg1);//再次压入
```

## 极速序列化
gdnet内部实现了极速序列化, 速度远超出protobuff 5-10倍, 在案例1测试中就采用了极速序列化适配器, 可以同步1万个cube, 如果用protobuff的话,只能同步2500个cube
内部的序列化已经有三个版本, 一个是之前的NetConvertOld字符串序列化,这个版本性能是非常糟糕的,性能远不及Newtonsoft.Json, 而第二版本的序列化NetConvertBinary二进制序列化则超越protobuff的性能, 体积也和protobuff一样, 为什么比protobuff快? protobuff内部实现还是使用的反射field.GetValue这种方法,而NetConvertBinary则是采用了dynamic动态语法实现的,在获取值和写值时比反射field.GetValue要快5倍. 这个NetConvertBinary版本已经超越protobuff了,为什么还要开发极速序列化NetConvertFast2? 主要还是为了框架的高性能处理.
NetConvertFast2极速序列化的使用:
1.要生成绑定类型, 在unity中有生成绑定类型工具, 也可以在这里生成:[绑定类型工具](https://gitee.com/leng_yue/fast2-build-tool)

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/fast2build.png" width = "414" height = "229" alt="图片名称" align=center />

```
public class Test //序列化的类型
{
    public int num;
    public string str;
}

static void Main(string[] args)
{
    NetConvertFast2.AddSerializeType3<Test>();//绑定Test为可序列化类型
    var seg = NetConvertFast2.SerializeObject(new Test());//序列化Test类
    var obj = NetConvertFast2.DeserializeObject<Test>(seg);//反序列化Test类
}
```
你也可以调用此api直接生成绑定类型

```
Fast2BuildToolMethod.Build(typeof(Test), AppDomain.CurrentDomain.BaseDirectory);//生成test绑定类型
Fast2BuildToolMethod.BuildArray(typeof(Test), AppDomain.CurrentDomain.BaseDirectory);//生成test数组绑定类型
Fast2BuildToolMethod.BuildGeneric(typeof(Test), AppDomain.CurrentDomain.BaseDirectory);//生成test泛型绑定类型
```
新版本中增加了运行时动态编译绑定类型

```
Fast2BuildMethod.DynamicBuild(BindingEntry.GetBindTypes());//动态编译指定的类型列表
```
详细信息请打开案例: GameDesigner\Example\SerializeTest\Scenes\example3.unity 查看

## ECS模块
ECS模块类似unity的gameObject->component模式, 在ecs中gameObject=entity, component=component, system类执行, ecs跟gameObject模式基本流程是一样的, 只是ecs中的组件可以复用, 而gameObject的component则不能复用, 在创建上万个对象时, gameObject就得重新new出来对象和组件, 而ecs调用Destroy时是把entity或component压入对象池, 等待下一次复用.实际上对象没有被释放,所以性能高于gameObject的原因


```
//ecs时间组件
public class TimerComponent : Component, IUpdate //继承IUpdate接口后就会每帧调用Update方法
{
    private DateTime dateTime;
    public override void Awake()
    {
        dateTime = DateTime.Now.AddSeconds(5);//在初始化时,把当前时间推到5秒后
    }
    public void Update()
    {
        if (DateTime.Now >= dateTime)//当5秒时间到, 则删除这个时间组件, 实际上是压入对象池
        {
            Destroy(this);
        }
    }
    public override void OnDestroy()//当销毁, 实际是压入对象池前调用一次
    {
    }
}

static void Main(string[] args)
{
    var entity = GSystem.Instance.Create<Entity>();//创建实体对象,这会在对象池中查询,如果对象池没有对象,则会new, 有则弹出entity
    entity.AddComponent<TimerComponent>();//添加时间组件,也是从对象池查询,没有则new, 有则弹出TimerComponent对象
    while (true)
    {
        Thread.Sleep(30);
        GSystem.Instance.Run();//每帧执行ecs系统
    }
}
```

## MVC模块
mvc模块:模型,控制,视图分离, mvc模块适应于帧同步游戏, model定义了对象字段,属性,事件, controller执行业务逻辑, view显示结果
在帧同步中, mvc是分离的, 各自处理各自的, 做到可以不相关的地步, 比如view卡住, controller还是一直执行, 互不影响!

热更新FieldCollection组件使用:当在热更新项目中, 字段无需使用Find各种查找, 使用FieldCollection组件即可自动帮你处理完成字段收集引用, 一键生成即可写你的功能代码
<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/hotfixFC.png" width = "1179" height = "685" alt="图片名称" align=center />

## ILRuntime热更新
网络协议传输类型必须在主工程定义! 那什么热更网络协议类型? 热更新网络协议类型必须新下载主工程apk替换旧的apk, 重新安装新的apk, 由于主工程的apk大小不是很大, 所有的资源都在ab文件里面! 所以是可以这样更新的
注意: 协议定义在热更新项目中将无法反序列化! 必须定义在主工程!

热更新案例文档:[案例2热更新](https://docs.qq.com/doc/DS3FXbERiUXZnWHVx)

## UNet&Mirror设计模式

<br>变量同步案例:Assets/GameDesigner/Example/Example1/Scenes/SyncVarDemo.unity</br>
<br>可以同步C#基础单元结构体: byte, sbyte, short, ushort, char, int, uint, float, long, ulong, double, DateTime, decimal, string类型</br>
<br>同步自定义结构体: 纯包含基元类型, 不需要任何额外处理</br>
<br>同步自定义结构体,包含类型字段: 需要重写Equals额外处理字段对等</br>
<br>同步类型:需要重写Equals额外处理字段对等</br>

<br>与场景内的玩家进行变量同步: 原理是检查字段值有没有改变, 改变了就会往服务器发送, 服务器转发给场景内的所有客户端, 以identiy值取到对应的对象, 进行变量设置, 达到变量同步效果!</br>
<br>客户端与服务器进行变量同步: 原理是检查字段值改变后, 发送字段的id和值到服务器, 服务器检查NetPlayer的变量管理列表取出对应的对象, 进行变量设置, 达到p2p变量同步效果</br>

<br>[SyncVar]//在字段定义这个特性, 则为玩家之间变量</br>
<br>[SyncVar(authorize = false)]//这是你实例化的网络物体, 其他玩家不能改变你的对象变量, 即使改变了也不会发生同步给其他玩家, 只能由自己控制变量变化后才会同步给其他玩家</br>
<br>[SyncVar(id = 1)]//这是p2p 客户端只与服务器的netplayer之间变量同步, 开发者要保证id必须是唯一的 详情请看案例1的Example1.Client类定义</br>

## 百万级别RPC小数据测试
这里我们测试了100万次从客户端到服务器的请求并响应, 所需要的时间是4.67秒

需要引用这些命名空间
```
using Net.Client;
using Net.Config;
using Net.Event;
using Net.Server;
using Net.Share;
using Net.System;
using System;
using System.Diagnostics;
using System.Threading;
```

```
internal class Program
{
    static Stopwatch stopwatch;

    static void Main(string[] args)
    {
        NDebug.BindLogAll(Console.WriteLine);

        Config.UseMemoryStream = true;//使用运行内存作为数据缓冲区
        BufferStreamShare.Size = 1024 * 1024 * 100;//服务器每个客户端可以缓存的数据大小

        //此处是服务器部分, 可以复制到另外一个控制台项目
        var server = new TcpServer();
        server.LimitQueueCount = 10000000;//测试小数据的快速性能, 可以设置这里, 默认限制在65536
        server.PackageLength = 10000000;//小数据包封包合包大小, 一次性能运送的小数据包数量
        server.PackageSize = 1024 * 1024 * 50;//接收缓存数据包的最大值, 如果超出则被丢弃
        server.AddAdapter(new Net.Adapter.SerializeAdapter3());//采用极速序列化进行序列化rpc数据模型
        server.AddAdapter(new Net.Adapter.CallSiteRpcAdapter<NetPlayer>(server));//采用极速调用rpc方法适配器
        server.Run();

        //此处是客户端部分, 可以复制到另外一个控制台项目
        var client = new TcpClient();
        client.LimitQueueCount = 10000000;
        client.PackageLength = 10000000;
        client.PackageSize = 1024 * 1024 * 50;
        client.AddAdapter(new Net.Adapter.SerializeAdapter3());
        client.AddAdapter(new Net.Adapter.CallSiteRpcAdapter(client));
        client.AddRpcHandle(new Program());
        client.Connect().Wait();

        client.SendRT(new byte[1]);//先进入服务器
        Thread.Sleep(500);

        stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 1000000; i++)
        {
            client.SendRT(NetCmd.LocalRT, 1, i);
            if (i % 10000 == 0)
                Thread.Sleep(50);
        }

        Console.ReadLine();
    }

    [Rpc(hash = 1)]
    void test(int i)
    {
        if (i % 10000 == 0)
            Console.WriteLine(i);
        if (i >= 999999)
        {
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}
```
## MMORPG多人游戏AOI九宫格同步
提供aoi九宫格同步模块, 服务器,客户端都可使用(使用时请在unity可视化调整九宫格网格或大小视图后,再将值修改到服务器,解决了服务器没有可视化图形的问题), 当万人同步时, 如果全部同步的话,带宽直接爆炸! 为了解决带宽问题, 使用了九宫格同步法, 只同步在9个格子之间的玩家或怪物, 这样就可以解决带宽的大多数问题. 详情请看自带的aoi案例

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/aoi.png" width = "1036" height = "663" alt="图片名称" align=center />

## Gcp可靠协议
 **gcp协议的实现原理：**
<br>gcp协议的4个关键函数Send，Update, Input，Receive组成：Send由用户调用，向gcp内部传递发送的数据，然后用户需要每帧调用Update进行数据更新，内部处理数据超时重传等更新。
用户需要通过OnSender事件进行socket.Send发送gcp数据，当对方socket通过socket.ReceiveFrom接收到数据后需要把数据传递给Input方法，然后调用Receive方法进行接收gcp的数据。</br>
<br> **gcp功能：**</br>
<br>流量控制：MTPS属性控制每秒可发送多少字节的数据</br>
<br>超时重传：RTO属性控制在多少毫秒如果没有收到确认帧，则进行重传</br>
<br>最大传输单元：MTU属性控制每次只能发送多少个字节</br>
<br>数据帧：每个数据帧会有frame标志(byte), package号(uint)，serialNo数据帧序号(int)，count数据长度(int)，总共头部为13字节</br>
<br>多帧确认：在一秒内可以发送很多数据帧，无需顺序，到达对方后会进行字典接收，然后返回Ack确认帧给客户端进行超时重传移除(不再重传)，如果Ack发送过程丢失，则客户端会重新发起数据帧，直到Ack到客户端。当对方接收到的帧数据package号大于当前packageLocal号时，放入字典等待即可，等真正的packageLocal号确认完成后，一拼调用。</br>

```
static void Main(string[] args)
{
    var gcp = new GcpKernel();
    var gcp1 = new GcpKernel();
    gcp.OnSender += (b) => {
        gcp1.Input(b);//socket.send(b);
    };
    gcp1.OnSender += (b) => {
        gcp.Input(b);//socket.send(b);
    };
    gcp.Send(new byte[1235522]);
    gcp1.Send(new byte[1235522]);
    while (true)
    {
        gcp.Update();
        gcp1.Update();
        if (gcp.Receive(out var buffer) != 0) 
        {
            Console.WriteLine($"接收到的数据:{buffer.Length}");
        }
        if (gcp1.Receive(out var buffer1) != 0)
        {
            Console.WriteLine($"接收到的数据:{buffer1.Length}");
        }
    }
}
```

## 常见问题总汇
这里是开发者遇到的问题, 我都会在这里详细写出来, 这样大家遇到的问题都可以先在这里查看

## 致谢

谢谢大家对我的支持，如果有其他问题，请加QQ群:825240544讨论

## TapTap游戏
<br>1.[士兵召唤](https://www.taptap.com/app/221847)</br>
<br>2.[我是一只鱼](https://www.taptap.com/app/220242)</br>
<br>3.[山海经吞食天地](https://www.taptap.com/app/207447)</br>
<br>4.[捍卫星球](https://www.taptap.com/app/170490)</br>
<br>5.[一介散修](https://www.taptap.com/app/234010)</br>
<br>6.[无限升级](https://www.taptap.cn/app/241449)</br>

## 支持本项目
您的支持就是我不懈努力的动力。打赏时请一定留下您的称呼
<br>感谢以下人员对gdnet捐款:</br>

<br>1 vsmile ¥ 10</br>
<br>2 南归 ¥ 10</br>
<br>3 王者心，懂么？ ¥ 10</br>
<br>4 郭少 ¥ 5000</br>
<br>5 思念天边的你 ¥ 52</br>
<br>6 娟子 ¥ 1000</br>
<br>7 Slarvens ¥ 30</br>
<br>8 达西莉莉 ¥ 200</br>
<br>9 扬神无敌 ¥ 100</br>
<br>10 29.8°C ¥ 30</br>
<br>11 走在冷风中. ¥ 1000</br>
<br>12 非非有非非无 ¥ 100</br>
<br>13 克里斯 ¥ 200</br>

<br>不留名的大佬们 微信总资助 ¥ 653</br>

<img src="https://gitee.com/leng_yue/GameDesigner/raw/master/pay.jpg" width = "600" height = "400" alt="图片名称" align=center />