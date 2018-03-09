//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Elmah;
//using Infrastructure.SqlStore;

//namespace SqlStore.Implementation
//{
//    public class MemberService: IMemberService
//    {
//        private readonly IMemberRepository memberRepository;

//        public MemberService(IMemberRepository memberRepository)
//        {
//            this.memberRepository = memberRepository;
//        }

//        public void SyncMembers(List<IMember> members)
//        {
//            foreach (var member in members)
//            {
//                member.Activated = true;
//                memberRepository.SaveOrUpdate(member);
//            }
//            DeactivateMembers(members);
//        }

//        private void DeactivateMembers(List<IMember> members)
//        {
//            IEnumerable<IMember> existingMembers = memberRepository.GetAll();
//            IEnumerable<IMember> deactivatedMembers = existingMembers.Where(ed => members.All(d => d.Samaccountname != ed.Samaccountname &&
//                d.EMail != ed.EMail));

//            foreach (var member in deactivatedMembers)
//            {
//                member.Activated = false;
//                memberRepository.SaveOrUpdate(member);
//            }
//        }

//    }
//}
