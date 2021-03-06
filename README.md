# ShareMemory
一个用 C# 实现的 No Sql 数据库 ， 也可以说是 分布式 缓存 ， 用于作为 集群 的 共享内存





ShareMemory 是 一个用 C# 实现的 No Sql 数据库 ， 也可以说是 分布式 缓存 ， 用于作为 集群 的 共享内存 。 

构建 集群 的 关键是 共享内存 。 ShareMemory 可以作为 集群 的 共享内存 ， 帮助 构建 集群 ， 这是 ShareMemory 的 第一 设计目标 。

事实上 ， 在过去的 十几年间 ， 利用 分布式缓存 来作为 共享内存 构建 Web 集群 ， 已经成为 事实上 的 做法 。

ShareMemory 设计目标中支持的 集群 包含 Web 集群 ， 分布式并行计算集群  等。

ShareMemory 支持 2 种 数据结构 ： 字典（Dictionary）  队列（Queue） 。

支持 6 大类 数据类型 ： Value Type ， string ， Simple Object ， Value Type 数组 ， string 数组 ， Simple Object 数组 。

可以将这 6 大类 数据类型 存放到 ShareMemory 。

Simple Object 是指 属性（Property）和 字段（Field） 类型 是 Value Type ， string 的 对象 ， 简单的说 ， 不支持 对象嵌套 。 这部分会在下面 序列化 的 部分详细介绍 。

ShareMemory 提供的 数据操作 都是 原子操作 ， 是 线程安全 的 。

解决方案 中 包含 6 个 项目：

Client ： 用于 Demo 的 Client

Server ： 用于 Demo 的 Server

ShareMemory ： ShareMemory 核心库 ， 用于 Server 端

ShareMemory.Client ： ShareMemory 客户端库 ， 用于 Client 端

ShareMemory.Serialization ： ShareMemory 序列化库 ， 用于 序列化

Test ： 用于测试 ShareMemory.Serialization 的 测试项目


ShareMemory 服务器端 在 App.config 中配置 字典 和 队列，在 AppSettings 中 通过 “ShareMemory.Dics” 和 “ShareMemory.Queues” 2 个 key 来配置 字典 和 队列 ， 如 

add key="ShareMemory.Dics" value="Dic1, Dic2, Dic3"  

add key="ShareMemory.Queues" value="Queue1, Queue2, Queue3"  

Dic1 Dic2 Dic3 表示要创建的 字典 ， Queue1 Queue2 Queue3 表示要创建的 队列 ， 字典名 队列名之间用 逗号 “,” 隔开 。 这样 ShareMemory Host 在启动时会创建 Dic1 Dic2 Dic3 3 个 字典 ， 和 Queue1 Queue2 Queue3 3 个 队列 。


ShareMemory 客户端 通过 ShareMemory.Client 库 提供的 Helper 类 ， Dic 类 ， Q 类 来 访问 ShareMemory 服务器端 。

Helper类提供 GetDic() 方法 ， 返回 Dic 对象 。 和 GetQ() 方法，返回 Q 对象 。

Dic 提供 Set(key, value) 方法 ， Get<T>(key) 方法 ， TryGet<T>(key out value) 方法 ， Remove(key) 方法 。 Set() 方法 新增键值对 或者 修改键值对的值 ， 如果 键值对 不存在，则新增键值对，如果键值对已存在，则更新值 。 Get() 方法从 Dic 取得值 ， 对于 引用类型 ， 如果 键值对 不存在 ， 则返回 null 。 TryGet() 方法也是从 Dic 取得值，通过 out value 参数返回， 若 键值对 不存在，则 Get() 方法返回值为 false 。 TryGet<T>() 方法是对 Value Type 设计的 ， 因为 Value Type 不能根据返回值为 null 来判断键值对在 Dic 中是否存在 。 Remove() 方法 移除 键值对 ， 如果 键值对 不存在 ， 也不会报错 。 


Q 提供 En() 方法 ， De<T> ， TryDe<T>(out value) 方法 。 En() 方法将 对象 放入 队列 ， De() 方法从 队列 取出对象 ， 对于 引用类型 ， 若返回 null ， 表示 队列 为空 。 TryDe() 方法也是从 队列 取出 对象 ， 通过 out value 参数返回 ， 若 队列 为空 ， TryDe() 方法返回值为 false 。 TryDe() 方法是对 Value Type 设计的 ， 因为 Value Type 不能根据返回值为 null 来判断 队列 是否为空 。 
  

接下来 说明 一下 序列化 的 格式 ：

序列化由 ShareMemory.Serialization 项目完成 ， 序列化格式 是 这样的 ：

比如 ， 有一个 Simple Object ， 包含有 1 个 int A 属性 ， 1 个 string B 字段 ， A = 2 , B = "Hello" ， 那么 ， 序列化产生这样一个字符串 ： 

“o 1 A1 21 B5 Hello”

把 这个 字符串 通过 Encoding.Utf8 转成 byte 数组 ， 就是 序列化 的 结果 了 。   

这个 字符串 的 开头 是 “o” ， 这表示 Simple Object 对象 ， 后面跟着 一个 空格 ， 空格 后面 的 “1” 表示 接下来 的 数据长度 是 1 个 字符 。 这个数据就是 后面的 “A” ， 这表示 A 属性的 属性名 ， “A” 后面 有一个 “1” ， 这表示 下一项 的 数据长度 是 1 ， 这个数据就是 后面的 “2” ， 这是 A 属性的 值 ， 以此类推 ， “2” 后面 紧跟着 的 “1” 是 下一项 的 数据长度 ， 这个数据就是 “B” 字符 ， 这表示 B 属性的 属性名 ， “B” 后面的 “5” 表示 下一项 的 数据长度 ， 这个数据就是 “Hello” 。 这样就完成了 对 这个 Simple Object 的 序列化 。   


大家可以看到 ， 对于 int 类型 ， 序列化 的 方式 是 ToString() ， 对于 string ， 就是 string 本身 ， 实际上 ， 目前 除了 DateTime 外 ， 其它的 Value Type 都是 以 ToString() 的方式来 序列化 ， DateTime 是 取 Ticks 属性 ， 当然 string 就是 string 本身 。    

如果是 单独 序列化 一个 Value Type 的值 ， 比如 int a = 2; 那么 就是 “1 2” 这样一个 字符串 ， 如上所述 ， “1” 表示 数据长度 ， “2” 表示 数据值 。    

对于 数组类型 ， 举个例子 ， 假设 有一个 数组 ， 放了 2 个 Simple Object 对象 ， 这个 Simple Object 对象 就是 上面说的 那个 ， 那 序列化 后的 字符串 是 这样的 ：   

“a 2 18 o 1 A1 21 B5 Hello18 o 1 A1 21 B5 Hello”

第一个字符 “a” 表示 数组 ， 这是 固定的 ， 后面跟一个 空格 ， 这也是固定的 。 空格后面是 “2” ， 表示 数组长度 ， 即 数组元素 的 个数 。 “2” 后面跟一个 空格 ， 这也是固定的 ， 空格后面是 “18” ， 表示 接下来 的 元素 的 长度 ， “18” 后面跟一个 空格 ， 这也是固定的 。 空格之后 就是 元素 的 内容 。 这个内容 ， 就是上面我们讲过的 Simple Object 序列化之后的 字符串 ， 这个 字符串 长度是 18 ， 前面的 “18” 就是指这个 。 以此类推 ， 第一个元素结束之后 ， 又是一个 “18” ， 这个 18 是指 第二个元素的长度 ， “18” 后面是空格 ， 空格 之后 就是 第二个元素 序列化之后的 字符串 。  

Value Type 数组 ， string 数组 的 原理 都包含在 上述 里了 ， 就不具体举例了 。

总之 ， ShareMemory.Serialization 可以支持 6 大类 数据类型 的 序列化 ： Value Type ， string ， Simple Object ， Walue Type 数组 ， string 数组 ， Simple Object 数组 。  

可以实际到 项目 里 运行 看一下 效果 就比较清楚了 。      ^ ^

ShareMemory.Serialization 只会序列化 公有 的 属性 和 字段 ， 并且需要在要序列化的 属性 和 字段 上 加上  [ S ]  标记  。

ShareMemory.Serialization 并不要求 序列化方 和 反序列化方 的 对象定义 在 语法 上 完全一致 ， 比如 序列化方 的 对象 有一个 A 属性 ， 反序列化方 可以用一个 A 字段 来 接收 A 属性 的 值 ， 只要 两者 的 名字 相同就行 。

ShareMemory.Serialization 可以作为一个 序列化库 单独使用 。  



ShareMemory 没有提供 对 数据操作 的 锁 机制（Lock） ， 因为 对 数据 的 锁机制 逻辑 比较 复杂 。 那么 ， 多个 客户端 线程 之间 怎么进行 通信协作 呢 ？ ShareMemory 提供了 与 数据无关 的 锁机制 。 Helper类 提供了 TryLock(lockName) 方法 和 UnLock(lockName, lockId) 方法 。 TryLock() 用来获取 锁 ， 参数 lockName 是 锁 的 名字 ， 参与协作 的 线程 间 可以 约定 一个 锁 的 名字 来 通信 。 TryLock() 方法 的 返回值 是 lockId ， 用来 标识 1 次 Lock ， 因为同一个 名字 的 锁 可能会多次 Lock 和 UnLock 。 UnLock 的时候需要 传入 lockId 参数 。 如果 TryLock() 方法返回的 lockId 是 null ， 则表示未成功获取锁 ， 客户端 可能需要再次 TryLock() 。

在 分布式系统 中 ， 因为一些原因 ， 可能会发生 锁 没有解锁就被 “遗弃” 的 情况 ， 比如 发起 锁定 的 客户端 线程 死掉 或者 掉线 了 ， 这样就会造成 “遗弃” 的 锁 。 这个 锁 就一直没人解 ， 就会造成 其它 线程 一直等待而不能正常运行 。 为了避免这种问题 ， ShareMemory 规定 锁 的 有效时间 是 1 分钟 ， 超过 1 分钟的锁会被系统 自动解锁 。 ShareMemory 每 30 秒 执行一次 回收锁 的 任务 ， 所以实际中 锁 的 最大有效时间 理论上 大约是 1 分 30 秒 。

这就是 ShareMemory 提供的 锁机制 ， 可以利用这个 锁机制 来 实现 多个 客户端 线程 间 的 通信协作 。 以此为基础 ， 开发者还可以实现各种丰富的线程间通信协作方式 。            

关于 锁 机制 ， 可以在 Client 项目中 查看 Demo 。  


接下来 再来 讨论 持久化 水平扩展 可用性 数据不丢失性  。

ShareMemory 不提供 持久化 。 持久化 仍然 交给 传统的 关系数据库 和 文件系统 。

ShareMemory 不提供 水平扩展 。 水平扩展 会 带来 性能损耗 。 当然这不是多有理由的理由 ， 啊哈哈 。

ShareMemory 不提供 可用性 。 开发者可以自己想办法解决 ， 比如 准备一台 备机 。

ShareMemory 不提供 数据不丢失性 。 ShareMemory 相当于 内存 ， 所以 不提供 数据不丢失性 。 这好像也不是什么理由 ， 哈哈哈哈 。


对于以上 ， 我们考虑过一些方案 ， 比较 简单经典 的 方案 是 主从热备 ， 但在具体设计的时候 ， 发现仍然有一些复杂的情况 。 比如 主从热备 ， 是 同步备 还是 异步备 ？ 同步备 程序比较简单 ， 但会带来 性能损耗 ， 对 在用主机 的 响应时间 产生影响 。 因为要把 每一笔 数据更新 操作 包含了 主机 和 备机 双份的 操作 ， 主机 备机 2 份操作合在一起 作为一个 操作 。 并且如果 备机 发生问题 会反过来 影响主机 。

那么 异步备 呢 ？ 异步备 可以把 数据更新操作 放到 一个 队列（Queue）里 ， 然后 由 另一个 线程 来 逐一 读取 队列 里的 操作 对 备机 执行 。 但 问题是顺序的执行这些操作 ， 这样才能 还原 主机 上的数据变化 。 这就导致 不能 多线程并行 执行 。 这样带来的问题是 ， 假如 服务器 的 CPU 是 4 核 ， 如果 更新 很频繁的话 ， 那么可能有 3 个 核 加入了 更新的工作 ， 热备 只有一个 线程 ， 最多只能利用 1 个核 ， 那么 更新记录 的 增长 会 大于 消费 ， 那么 队列 里的 更新记录 会 越来越多 ， 堆积起来 。 把 更新记录 写到 文件 里 也没用 ， 文件 也会增长 。 事实上 ， Sql Server 的 Always On 也存在 Log 膨胀 的 问题 。 


对于 以上的问题 ， 如果反过来 ， ShareMemory只提供一个内存模型 ， 并不 一刀切 的 负责 可用性 数据一致性 数据完整性 ， 另一方面 ，开发者的程序自己实现以下 3 件事：

1 数据何时持久化（哪些数据需要持久化），就像我们编辑文档会不定期随时保存一样

2 主机挂掉时，新的主机启动时需要预先加载哪些数据，如常用数据，如 User Profile

3 一些重要数据的备份，比如 定期 不定期 的 数据同步 快照

我想，这样，事情就简单了。

ShareMemory 提供 读写数据 的 API 即可 。

ShareMemory 提供一个内存模型，持久化 仍然 交给 传统的 关系数据库 文件系统 等 。


对于 集群 负载均衡 可用性 数据完整性（不丢失性） ， 我们可以参考 Windows NLB ， Windows 故障转移集群 ， Sql Server Log Shipping ， Sql Server Always On ， 然鹅 。  

对于 分布式缓存 分布式消息 的 集群 可用性 数据备份 数据同步 数据恢复 ， 我们可以参考 Redis RabbitMQ ， 然鹅 。



根据网上的测评结果 ， 固态硬盘 的 连续读取速度 可以达到 1800M/s 以上 （参考 http://ssd.zol.com.cn/608/6082302.html ） 。 我们可以来假想评估一个 使用场景 。 比如 ， 以 门户网站 的 场景 为例 ， 假设有 100 万 人同时在线 ， 用 ShareMemory 来存储 User Profile 的话 ， 假如每个用户的 User Profile 大小 是 1 KB ， 那么 ， 100 万个 用户的 User Profile 占用的空间就是 1 G 。 如果有 1 亿个用户的话 ， 那占用的空间就是 100 G 。 在 操作系统 虚拟内存 的 支持 下 ， 32 G 内存 + 120 G 固态硬盘 应该会有不错的表现 。 或者 ， 16 G 内存 + 120 G 固态硬盘 ， 8 G 内存 + 120 G 固态硬盘 也许表现都会很好 。  


ShareMemory 的 远程通信 采用 MessageRPC 实现 ， MessageRPC 是我写的另一个项目 ：  https://github.com/kelin-xycs/MessageRPC       









































