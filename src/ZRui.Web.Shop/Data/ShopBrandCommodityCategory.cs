using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ZRui.Web
{
    /// <summary>
    /// ����Ʒ�Ƶ���Ʒ����
    /// </summary>
    public class ShopBrandCommodityCategory : EntityBase
    {
        /// <summary>
        /// ����������Ʒ��
        /// </summary>
        [ForeignKey("ShopBrandId")]
        public ShopBrand ShopBrand { get; set; }
        /// <summary>
        /// ����������Ʒ�Ƶ�Id
        /// </summary>
        public int ShopBrandId { get; set; }
        /// <summary>
        /// ��Ʒ��������
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// ��ʶ
        /// </summary>
        public virtual string Flag { get; set; }
        /// <summary>
        /// ˵��
        /// </summary>
        public virtual string Detail { get; set; }
        /// <summary>
        /// �������ϼ�����
        /// </summary>
        [ForeignKey("PId")]
        public virtual ShopBrandCommodityCategory Parent { get; set; }
        /// <summary>
        /// �������ϼ�����Id
        /// </summary>
        public virtual int? PId { get; set; }
        /// <summary>
        /// ͼ�꣬���߽з���
        /// </summary>
        public virtual string Ico { get; set; }
        /// <summary>
        /// ����Ȩ��
        /// </summary>
        public virtual float OrderWeight { get; set; }
        /// <summary>
        /// ��ǩ������ҳ��seo
        /// </summary>
        public virtual string Tags { get; set; }
        /// <summary>
        /// �ؼ��֣�����ҳ��seo
        /// </summary>
        public virtual string Keywords { get; set; }
        /// <summary>
        /// ����������ҳ��seo
        /// </summary>
        public virtual string Description { get; set; }
    }
}