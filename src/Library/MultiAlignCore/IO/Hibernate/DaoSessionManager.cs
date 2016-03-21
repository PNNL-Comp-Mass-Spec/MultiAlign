namespace MultiAlignCore.IO.Hibernate
{
    using System;
    using NHibernate;

    public class DaoSessionManager : IDisposable
    {
        readonly ISession currentSession;

        public DaoSessionManager()
        {
            this.currentSession = NHibernateUtil.GetOrCreateSession();
        }

        public void Dispose()
        {
            this.currentSession.Flush();
            this.currentSession.Close();
            this.currentSession.Dispose();
        }
    }
}
