
using System;



public class Parser {
	const int _EOF = 0;
	const int _a = 1;
	const int _b = 2;
	const int _c = 3;
	const int _d = 4;
	const int _e = 5;
	const int _f = 6;
	const int _g = 7;
	const int _h = 8;
	const int _i = 9;
	const int maxT = 10;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Test() {
		A();
		B();
		Expect(7);
		C();
		Expect(7);
		D();
	}

	void A() {
		if (la.kind == 1) {
			Get();
		} else if (StartOf(1)) {
			while (la.kind == 5) {
				Get();
			}
			if (la.kind == 6) {
				Get();
			}
		} else SynErr(11);
	}

	void B() {
		while (la.kind == 2) {
			Get();
		}
		if (la.kind == 3) {
			Get();
		}
		if (la.kind == 4) {
			Get();
		} else if (la.kind == 0 || la.kind == 7) {
		} else SynErr(12);
	}

	void C() {
		A();
		B();
	}

	void D() {
		if (StartOf(2)) {
			C();
		} else if (la.kind == 8) {
			Get();
		} else SynErr(13);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Test();

    Expect(0);
	}
	
	bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x},
		{T,x,T,T, T,T,T,T, x,x,x,x},
		{T,T,T,T, T,T,T,x, x,x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
  public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text
  
	public void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "a expected"; break;
			case 2: s = "b expected"; break;
			case 3: s = "c expected"; break;
			case 4: s = "d expected"; break;
			case 5: s = "e expected"; break;
			case 6: s = "f expected"; break;
			case 7: s = "g expected"; break;
			case 8: s = "h expected"; break;
			case 9: s = "i expected"; break;
			case 10: s = "??? expected"; break;
			case 11: s = "invalid A"; break;
			case 12: s = "invalid B"; break;
			case 13: s = "invalid D"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}

