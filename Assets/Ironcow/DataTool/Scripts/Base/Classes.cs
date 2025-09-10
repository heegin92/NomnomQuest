// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework © 2025 Ironcow Studio
// Distributed via Gumroad under a paid license
// 
// 🔐 This file is part of a licensed product. Redistribution or sharing is prohibited.
// 🔑 A valid license key is required to unlock all features.
// 
// 🌐 For license terms, support, or team licensing, visit:
//     https://ironcowstudio.duckdns.org/ironcowstudio.html
// ─────────────────────────────────────────────────────────────────────────────


using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Ironcow.Synapse.Core;

[Serializable]
public partial class UserInfo : BaseModel
{
    class ObserverWrapper<T>
    {
        public UnityEngine.Object target;
        public Action<T> callback;

        public ObserverWrapper(UnityEngine.Object target, Action<T> callback)
        {
            this.target = target;
            this.callback = callback;
        }

        public bool IsValid => target != null;
    }

    public static UserInfo myInfo { get => DataManager.instance.userInfo; set => DataManager.instance.userInfo = value; }
    private static readonly Dictionary<string, MemberInfo> memberCache = new();
    private readonly Dictionary<string, List<object>> typedObservers = new();

    public void SetValue<T>(Expression<Func<T>> memberSelector, T value)
    {
        var member = GetMemberInfo(memberSelector);
        if (member == null) return;

        var current = GetValue(member);
        if (Equals(current, value)) return; // 변경 없으면 skip

        switch (member)
        {
            case FieldInfo f: f.SetValue(this, value); break;
            case PropertyInfo p: p.SetValue(this, value); break;
        }

        // 옵저버 호출
        if (typedObservers.TryGetValue(member.Name, out var list))
        {
            var toRemove = new List<object>();

            foreach (var obj in list)
            {
                if (obj is ObserverWrapper<T> wrapper)
                {
                    if (!wrapper.IsValid)
                    {
                        toRemove.Add(obj); // Destroy된 대상
                        continue;
                    }

                    wrapper.callback?.Invoke(value);
                }
            }

            // 파괴된 오브젝트 구독 제거
            foreach (var r in toRemove)
                list.Remove(r);
        }
    }

    private MemberInfo GetMemberInfo<T>(Expression<Func<T>> expr)
    {
        string key = expr.ToString(); // 캐시 키로 사용
        if (memberCache.TryGetValue(key, out var cached))
            return cached;

        MemberInfo member = null;
        if (expr.Body is MemberExpression m) member = m.Member;
        else if (expr.Body is UnaryExpression u && u.Operand is MemberExpression um) member = um.Member;

        if (member != null) memberCache[key] = member;
        return member;
    }

    private object GetValue(MemberInfo member)
    {
        return member switch
        {
            FieldInfo f => f.GetValue(this),
            PropertyInfo p => p.GetValue(this),
            _ => null
        };
    }

    public void Subscribe<T>(UnityEngine.Object target, Expression<Func<T>> expr, Action<T> callback)
    {
        var member = GetMemberInfo(expr);
        if (member == null) return;

        var key = member.Name;

        if (!typedObservers.TryGetValue(key, out var list))
            typedObservers[key] = list = new List<object>();

        list.Add(new ObserverWrapper<T>(target, callback));
    }
}

