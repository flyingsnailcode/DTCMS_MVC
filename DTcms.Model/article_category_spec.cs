using System;
namespace DTcms.Model
{
    /// <summary>
    /// 类别与规格关系表
    /// </summary>
    [Serializable]
    public partial class article_category_spec
    {
        public article_category_spec()
        { }
        #region Model
        private int _id;
        private int _category_id = 0;
        private int _spec_id = 0;
        /// <summary>
        /// 自增ID
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 分类ID
        /// </summary>
        public int category_id
        {
            set { _category_id = value; }
            get { return _category_id; }
        }
        /// <summary>
        /// 规格ID
        /// </summary>
        public int spec_id
        {
            set { _spec_id = value; }
            get { return _spec_id; }
        }
        #endregion
    }
}