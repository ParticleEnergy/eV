// Copyright (c) ParticleEnergy. All rights reserved.
// Licensed under the Apache license. See the LICENSE file in the project root for full license information.

namespace eV.Framework.Server.Options;

public class RedisOption
{
    public string[] Address { get; set; } = Array.Empty<string>();
    public string? User { get; set; } = string.Empty;

    /// <summary>
    /// 一个字符串值，指定连接 Redis 服务器所需的密码。默认值为 null。
    /// </summary>
    public string? Password { get; set; } = string.Empty;

    /// <summary>
    /// 一个整数值，指定默认数据库的编号。默认值为 0。
    /// </summary>
    public int? Database { get; set; } = null;

    /// <summary>
    /// 一个整数值，指定在没有交互的情况下维持 TCP 连接的时间。默认值为 -1（表示禁用）。
    /// </summary>
    public int? Keepalive { get; set; } = null;

    /// <summary>
    /// 异步操作超时时间
    /// </summary>
    public int? AsyncTimeout { get; set; } = null;

    /// <summary>
    /// 同步操作超时时间
    /// </summary>
    public int? SyncTimeout { get; set; } = null;

    /// <summary>
    /// 一个布尔值，指定是否使用 SSL/TLS 加密连接。默认值为 false。
    /// </summary>
    public bool? Ssl { get; set; } = null;

    /// <summary>
    /// 一个字符串值，指定 SSL/TLS 连接的主机名。默认值为 null。
    /// </summary>
    public string? SslHost { get; set; } = null;

    /// <summary>
    /// 允许连接器执行管理员命令
    /// </summary>
    public bool? AllowAdmin { get; set; } = null;

    public int VersionMajor { get; set; } = 5;
    public int VersionMinor { get; set; } = 0;
    public int VersionBuild { get; set; } = 0;
}
