import java.util.Scanner;

public class Main {
	static int maxMemory = 64;
	static int newMemory;
	static int currentMemory = 0;
	static int mem;
	static Scanner input = new Scanner(System.in);
	
	public static void main(String[] args) {
		addMemory();
		while(currentMemory <= maxMemory){
			if(currentMemory + mem <= maxMemory) {
				currentMemory += mem;
				addMemory();
			} else {
				fullMemory();
				return;
			}
		}	
	}

	public static void addMemory() {
		System.out.print("Max Memory = " + maxMemory + "\nCurrent Memory = " + currentMemory + "\nEnter amount of memory to add : ");
		mem = input.nextInt();	
        }
		
	public static void fullMemory() {
		System.out.print("Max Memory = " + maxMemory + "\nCurrent Memory = " + currentMemory + "\nMEMORY FULL!\n");
	}
}
