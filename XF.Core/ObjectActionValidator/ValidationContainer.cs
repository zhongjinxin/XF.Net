using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XF.Core.Extensions;
using XF.Entity.DomainModels.System;

namespace XF.Core.ObjectActionValidator
{
    public static class ValidationContainer
    {
        public static IServiceCollection UseMethodsModelParameters(this IServiceCollection services)
        {
            //登陆方法校验参数,只验证密码与用户名
            ValidatorModel.Login.Add<LoginInfo>(x => new { x.Password, x.UserName, x.VerificationCode, x.UUID });

            //只验证LoginInfo的密码字段必填
            ValidatorModel.LoginOnlyPassWord.Add<LoginInfo>(x => new { x.Password });

            return services;
        }
        /// <summary>
        ///  普通属性校验
        /// 方法上添加[ObjectGeneralValidatorFilter(ValidatorGeneral.xxx)]即可进行参数自动验证
        /// ValidatorGeneral为枚举(也是方法的参数名)，自己需要校验的参数在枚举上添加
        /// ValidatorGeneral.xxx.Add() 配置自己的验证规则
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection UseMethodsGeneralParameters(this IServiceCollection services)
        {
            //配置用户名最多30个字符
            ValidatorGeneral.UserName.Add("用户名", 30);

            //方法参数名为newPwd，直接在方法加上[ObjectGeneralValidatorFilter(ValidatorGeneral.NewPwd)]进行参数验证
            //如果newPwd为空会提示：新密码不能为空
            //6,50代表newPwd参数最少6个字符，最多50个符
            //其他需要验证的参数同样配置即可
            ValidatorGeneral.NewPwd.Add("新密码", 6, 50);

            //如果OldPwd为空会提示：旧密码不能为空
            ValidatorGeneral.OldPwd.Add("旧密码");

            //校验手机号码格式
            ValidatorGeneral.PhoneNo.Add("手机号码", (object value) =>
            {
                ObjectValidatorResult validatorResult = new ObjectValidatorResult(true);
                if (!value.ToString().IsPhoneNo())
                {
                    validatorResult = validatorResult.Error("请输入正确的手机号码");
                }
                return validatorResult;
            });

            //测试验证字符长度为6-10
            ValidatorGeneral.Local.Add("所在地", 6, 10);

            //测试验证数字范围
            ValidatorGeneral.Qty.Add("存货量", ParamType.Int, 200, 500);

            return services;
        }
    }
    public enum ValidatorModel
    {
        Login,
        LoginOnlyPassWord//只验证密码
    }

    public enum ValidatorGeneral
    {
        UserName,
        OldPwd,
        NewPwd,
        PhoneNo,
        Local,//测试验证字符长度
        Qty//测试 验证值大小
    }
}
