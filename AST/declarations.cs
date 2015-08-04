﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST {

    public class Decln : ExternDecln {
        public enum SCS {
            AUTO,
            STATIC,
            EXTERN,
            TYPEDEF,
        }

        public Decln(String name, SCS scs, ExprType type, Expr init) {
            decln_name = name;
            decln_scs  = scs;
            decln_type = type;
            decln_init = init;
        }

        public override String ToString() {
            String str = "[" + decln_scs.ToString() + "] ";
            str += decln_name;
            str += " : " + decln_type.ToString();
            return str;
        }

        public void CGenExternDecln(Env env, CGenState state) {
            state.CGenExpandStackTo(env.GetStackOffset(), ToString());
            if (decln_init.type.kind != ExprType.Kind.VOID) {
                // need initialization

                Env.Entry entry = env.Find(decln_name);
                switch (entry.kind) {
                case Env.EntryKind.STACK:
                    // %eax = <decln_init>
                    decln_init.CGenValue(env, state);

                    // -<offset>(%ebp) = %eax
                    state.MOVL(Reg.EAX, -entry.offset, Reg.EBP);

                    break;
                case Env.EntryKind.GLOBAL:
                    // TODO : extern decln global
                    break;
                case Env.EntryKind.ENUM:
                case Env.EntryKind.FRAME:
                case Env.EntryKind.NOT_FOUND:
                case Env.EntryKind.TYPEDEF:
                default:
                    throw new NotImplementedException();
                }


            }
        }

        private readonly String     decln_name;
        private readonly SCS        decln_scs;
        private readonly ExprType   decln_type;
        private readonly Expr       decln_init;
    }

    public class InitList : Expr {
        public InitList(List<Expr> _exprs) :
            base(new TInitList()) {
            initlist_exprs = _exprs;
        }
        public readonly List<Expr> initlist_exprs;
    }
}
