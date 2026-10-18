using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Project
{
    public partial class Form1 : Form
    {
        //- - - VARS - - -
        const char DELIM = ',';

        // Vars - Consts
        const int MINUTES_IN_HOUR = 60;
        const double TAX_RATE = 0.0785;

        // Vars - Login
        string[,] tableUsers; // = {username, password, phone number}
        int indexUserRow = -1; // tableUsers[indexUserRow, #] to get the user's info. (-1 only before valid login)

        // Vars - Service choices - Listboxes
        int stylist_choice = -1;
        int haircut_choice = -1;
        int treatment_choice = -1;
        int addOns_choice = -1;

        // Vars - Time choices - Listboxes
        const string FILE_BOOKINGS = "Bookings.txt";
        int time_choice = -1;
        string[,] tableBookings;

        //Vars - User In - Textboxes
        const string STARS = "****";

        string name = "";
        string phoneNumber = "";
        string email = "";
        string cardNumber = "";
        string finalCardNumber = "";

        // Vars - Totals - Labels
        int total_time = 0;
        double total_cost = 0.00;

        double total_tax = 0.00;

        // Vars - Arrays
        // All times are in min
        // All costs are in $0.00
        // {Service, Time, Costs}
        string[,] stylists_Time_And_Costs = new string[0, 0]; //{Stylists , Time,  Costs}
        string[,] haircuts_Time_And_Costs = new string[0, 0]; //{Cuts , Time,  Costs}
        string[,] treatments_Time_And_Costs = new string[0, 0]; //{Treatments, Time, Costs}
        string[,] addOns_Time_And_Costs = new string[0, 0]; //{ Add-Ons , Time , Costs}

        public Form1()
        {
            InitializeComponent();

        }

        // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        // = FORM LOAD BEGINS HERE =
        // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        private void Form1_Load(object sender, EventArgs e)
        {
            // = = = = = = = = = = = = = = = = = = = = = = = = = = = =
            //Load login info into the 2D array, tableUsers

            //- VARS -
            const string FILE_USERS = "login.txt";
            const string FILE_STYLISTS = "Data_Stylists.txt";
            const string FILE_HAIRCUTS = "Data_Haircuts.txt";
            const string FILE_TREATMENTS = "Data_Treatments.txt";
            const string FILE_ADDONS = "Data_Addons.txt";

            string line = "";
            string[] lineSplit;

            int row = 0;
            int col = 0;

            int rowCount = 0;
            int colCount = 0;

            //- OPEN -
            FileStream fsUsers = new FileStream(FILE_USERS, FileMode.Open, FileAccess.Read);
            StreamReader srUsers = new StreamReader(fsUsers);

            //- FIND TABLE SIZES -
            //Find number of rows in file
            rowCount = File.ReadAllLines(FILE_USERS).Length;

            //Find minimum columns needed
            while (line != null)
            {
                lineSplit = line.Split(",");

                if (colCount < lineSplit.Length)
                {
                    colCount = lineSplit.Length;
                }

                line = srUsers.ReadLine();
            }

            //Reset position in file to begining of file
            fsUsers.Seek(0, SeekOrigin.Begin); //Go back to start of file

            //- SET TABLE SIZE -
            tableUsers = new string[rowCount, colCount];

            //- READ -
            line = srUsers.ReadLine(); //Load 1st line into variable again
            while (line != null)
            {
                lineSplit = line.Split(","); //Split line before reading again, cause might be null after reading

                for (col = 0; col < lineSplit.Length; col++)
                {
                    tableUsers[row, col] = lineSplit[col];
                }

                line = srUsers.ReadLine();
                row++;
            }

            //- CLOSE -
            srUsers.Close();
            fsUsers.Close();

            // = = = = = = = = = = = = = = = = = = = = = = = = = = = =
            //- - - READ THE STUFF FOR THE LISTBOXES - - -
            //- OPEN FILE_STYLISTS -
            if (!File.Exists(FILE_STYLISTS))
            {// if does not exist
                stylists_Time_And_Costs = new string[,] {
                    { "Error", "0", "0"},
                    { "loading", "0", "0"},
                    { "file", "0", "0"}
                };
            }
            else
            {// if exists
                FileStream fsStylists = new FileStream(FILE_STYLISTS, FileMode.Open, FileAccess.Read);
                StreamReader srStylists = new StreamReader(fsStylists);

                //- FIND TABLE SIZES -
                // Find cols
                line = srStylists.ReadLine();
                lineSplit = line.Split(DELIM);
                colCount = lineSplit.Length;

                // Find # rows
                rowCount = 0;
                while (line != null)
                {// Read thru, and count rows
                    rowCount++;
                    line = srStylists.ReadLine();
                }
                fsStylists.Seek(0, SeekOrigin.Begin);

                // Set size of 2D array
                stylists_Time_And_Costs = new string[rowCount, colCount];

                // Fill 2D array
                line = srStylists.ReadLine();
                row = 0;
                while (line != null)
                {
                    lineSplit = line.Split(DELIM);

                    for (col = 0; col < lineSplit.Length; col++)
                    {
                        stylists_Time_And_Costs[row, col] = lineSplit[col];
                    }

                    line = srStylists.ReadLine();
                    row++;
                }

                //- CLOSE -
                srStylists.Close();
                fsStylists.Close();
            }

            //- OPEN FILE_HAIRCUTS -
            if (!File.Exists(FILE_HAIRCUTS))
            {// if does not exist

                haircuts_Time_And_Costs = new string[,] {
                    { "Error", "0", "0"},
                    { "loading", "0", "0"},
                    { "file", "0", "0"}
                };
            }
            else
            {// if exists
                FileStream fsHaircuts = new FileStream(FILE_HAIRCUTS, FileMode.Open, FileAccess.Read);
                StreamReader srHaircuts = new StreamReader(fsHaircuts);

                // Find cols
                line = srHaircuts.ReadLine();
                lineSplit = line.Split(DELIM);
                colCount = lineSplit.Length;

                // Find # rows
                rowCount = 0;
                while (line != null)
                {// Read thru, and count rows
                    rowCount++;
                    line = srHaircuts.ReadLine();
                }
                fsHaircuts.Seek(0, SeekOrigin.Begin);

                // Set size of 2D array
                haircuts_Time_And_Costs = new string[rowCount, colCount];

                // Fill 2D array
                line = srHaircuts.ReadLine();
                row = 0;
                while (line != null)
                {
                    lineSplit = line.Split(DELIM);

                    for (col = 0; col < lineSplit.Length; col++)
                    {
                        haircuts_Time_And_Costs[row, col] = lineSplit[col];
                    }

                    line = srHaircuts.ReadLine();
                    row++;
                }

                //- CLOSE -
                srHaircuts.Close();
                fsHaircuts.Close();
            }

            //- OPEN FILE_TREATMENTS -
            if (!File.Exists(FILE_TREATMENTS))
            {// if does not exist
                treatments_Time_And_Costs = new string[,] {
                    { "Error", "0", "0"},
                    { "loading", "0", "0"},
                    { "file", "0", "0"}
                };
            }
            else
            {// if exists
                FileStream fsTreatments = new FileStream(FILE_TREATMENTS, FileMode.Open, FileAccess.Read);
                StreamReader srTreatments = new StreamReader(fsTreatments);

                // Find cols
                line = srTreatments.ReadLine();
                lineSplit = line.Split(DELIM);
                colCount = lineSplit.Length;

                // Find # rows
                rowCount = 0;
                while (line != null)
                {// Read thru, and count rows
                    rowCount++;
                    line = srTreatments.ReadLine();
                }
                fsTreatments.Seek(0, SeekOrigin.Begin);

                // Set size of 2D array
                treatments_Time_And_Costs = new string[rowCount, colCount];

                // Fill 2D array
                line = srTreatments.ReadLine();
                row = 0;
                while (line != null)
                {
                    lineSplit = line.Split(DELIM);

                    for (col = 0; col < lineSplit.Length; col++)
                    {
                        treatments_Time_And_Costs[row, col] = lineSplit[col];
                    }

                    line = srTreatments.ReadLine();
                    row++;
                }

                //- CLOSE -
                srTreatments.Close();
                fsTreatments.Close();
            }

            //- OPEN FILE_ADDONS -
            if (!File.Exists(FILE_ADDONS))
            {// if does not exist
                addOns_Time_And_Costs = new string[,] {
                    { "Error", "0", "0"},
                    { "loading", "0", "0"},
                    { "file", "0", "0"}
                };
            }
            else
            {// if exists
                FileStream fsAddons = new FileStream(FILE_ADDONS, FileMode.Open, FileAccess.Read);
                StreamReader srAddons = new StreamReader(fsAddons);

                // Find cols
                line = srAddons.ReadLine();
                lineSplit = line.Split(DELIM);
                colCount = lineSplit.Length;

                // Find # rows
                rowCount = 0;
                while (line != null)
                {// Read thru, and count rows
                    rowCount++;
                    line = srAddons.ReadLine();
                }
                fsAddons.Seek(0, SeekOrigin.Begin);

                // Set size of 2D array
                addOns_Time_And_Costs = new string[rowCount, colCount];

                // Fill 2D array
                line = srAddons.ReadLine();
                row = 0;
                while (line != null)
                {
                    lineSplit = line.Split(DELIM);

                    for (col = 0; col < lineSplit.Length; col++)
                    {
                        addOns_Time_And_Costs[row, col] = lineSplit[col];
                    }

                    line = srAddons.ReadLine();
                    row++;
                }

                //- CLOSE -
                srAddons.Close();
                fsAddons.Close();
            }

            // = = = = = = = = = = = = = = = = = = = = = = = = = = = =
            //- - - READ IN BOOKING INFO - - -
            //- OPEN -
            if (!File.Exists(FILE_BOOKINGS))
            {// if does not exist
                tableBookings = new string[,] {
                    { DateTime.Today.ToString("d")},
                };
            }
            else
            {// if exists
                FileStream fsBookings = new FileStream(FILE_BOOKINGS, FileMode.Open, FileAccess.Read);
                StreamReader srBookings = new StreamReader(fsBookings);

                // Find # cols
                line = srBookings.ReadLine();
                lineSplit = line.Split(DELIM);
                colCount = lineSplit.Length;

                // Find # rows
                rowCount = 0;
                while (line != null)
                {// Read thru, and count rows
                    rowCount++;
                    line = srBookings.ReadLine();
                }
                fsBookings.Seek(0, SeekOrigin.Begin);

                // Set size of 2D array
                tableBookings = new string[rowCount, colCount];

                // Fill 2D array
                line = srBookings.ReadLine();
                row = 0;
                while (line != null)
                {
                    lineSplit = line.Split(DELIM);

                    for (col = 0; col < lineSplit.Length; col++)
                    {
                        tableBookings[row, col] = lineSplit[col];
                    }

                    line = srBookings.ReadLine();
                    row++;
                }

                //- CLOSE -
                srBookings.Close();
                fsBookings.Close();
            }

            //- - - POPULATE LISTBOXES - - -
            //(Listboxes will be our user input for the menu portion.)

            //Stylists
            lbStylists.Items.Clear();
            for (int i = 0; i < stylists_Time_And_Costs.GetLength(0); i++)
            {
                lbStylists.Items.Add(stylists_Time_And_Costs[i, 0]);
            }

            //Haircuts
            lbHaircuts.Items.Clear();
            for (int i = 0; i < haircuts_Time_And_Costs.GetLength(0); i++)
            {
                lbHaircuts.Items.Add(haircuts_Time_And_Costs[i, 0]);
            }

            //Treatments
            lbTreatments.Items.Clear();
            for (int i = 0; i < treatments_Time_And_Costs.GetLength(0); i++)
            {
                lbTreatments.Items.Add(treatments_Time_And_Costs[i, 0]);
            }

            //Addons
            lbAddons.Items.Clear();
            for (int i = 0; i < addOns_Time_And_Costs.GetLength(0); i++)
            {
                lbAddons.Items.Add(addOns_Time_And_Costs[i, 0]);
            }

            // = = = DATE HANDELING (ALBERT'S PART) = = =
            const int HOURS_OFFSET = 12;
            string dateString;
            string[] dateStringSplit;
            DateTime dateFromFile = DateTime.Today;
            DateTime aptSlot;
            int hour;

            for (row = 0; row < tableBookings.GetLength(0); row++)
            {
                dateString = tableBookings[row, 0].Trim();
                dateFromFile = DateTime.Parse(dateString); //Computer doesn't like this

                if (tableBookings.GetLength(1) > 1)
                {// if there're values in the col other than date
                    for (col = 1; col < tableBookings.GetLength(1); col++)
                    {
                        if (tableBookings[row, col] == "0")
                        {// if booking empty (0), display them in combo box
                            hour = col + HOURS_OFFSET; // start at 1pm and increment by an hour
                            aptSlot = new DateTime(dateFromFile.Year, dateFromFile.Month, dateFromFile.Day, hour, 0, 0);
                            cbDates.Items.Add(aptSlot.ToString("MM/dd hh:00 tt"));
                        }
                    }
                    dateFromFile = dateFromFile.AddDays(1); //go to next day
                }
            }
        }// END OF FORM LOAD

        // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        // = = = = = = METHODS = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        /* Methods List
         * Format: data_type Name(any vars the method requires)
         * - void UpdateTotals() - handels updating totals from listbox values
         */

        //Update the totals from the listboxes
        public void UpdateTotals()
        {// Recalculates the totals and update values; Use when user changes a listbox

            //Clear Text from GUI output elements
            lblChosenStylist.Text = "";
            lblChosenHaircut.Text = "";
            lblChosenTreatment.Text = "";
            lblChosenAddons.Text = "";

            lblTotalTime.Text = "";
            lblTotalCost.Text = "";

            lblCostTreatments.Text = "";
            lblCostAddons.Text = "";

            //Reset totals
            total_time = 0;
            total_cost = 0.00;

            // - - - Accumulate Totals - - -
            // Haircuts
            for (int i = 0; i < haircuts_Time_And_Costs.GetLength(0); i++)
            {
                if (haircut_choice == i)
                {
                    lblChosenHaircut.Text += String.Format("{0}", haircuts_Time_And_Costs[i, 0]); // Output
                    total_time += int.Parse(haircuts_Time_And_Costs[i, 1]); // Time
                    total_cost += double.Parse(haircuts_Time_And_Costs[i, 2]); // Cost

                    lblCostHaircut.Text = (haircuts_Time_And_Costs[i, 2]);
                }
            }

            // Treatments
            foreach (int i in lbTreatments.SelectedIndices)

            {
                lblChosenTreatment.Text += String.Format("{0}\n", treatments_Time_And_Costs[i, 0]); // Output
                total_time += int.Parse(treatments_Time_And_Costs[i, 1]); // Time
                total_cost += double.Parse(treatments_Time_And_Costs[i, 2]); // Cost

                lblCostTreatments.Text += String.Format("{0}\n", double.Parse(treatments_Time_And_Costs[i, 2]).ToString("C2"));
            }

            // Add-Ons
            foreach (int i in lbAddons.SelectedIndices)

            {
                lblChosenAddons.Text += String.Format("{0}\n", addOns_Time_And_Costs[i, 0]); // Output
                total_time += int.Parse(addOns_Time_And_Costs[i, 1]); // Time
                total_cost += double.Parse(addOns_Time_And_Costs[i, 2]); // Cost

                lblCostAddons.Text += String.Format("{0}\n", double.Parse(addOns_Time_And_Costs[i, 2]).ToString("C2"));
            }

            // Stylists
            // In case we have rates per stylists based on how long service takes
            for (int i = 0; i < stylists_Time_And_Costs.GetLength(0); i++)
            {
                //if choice = chosen array index, add to total
                if (stylist_choice == i)
                {
                    lblChosenStylist.Text += String.Format("{0}\n", stylists_Time_And_Costs[i, 0]); // Output
                    //total_cost += total_time * double.Parse(stylists_Time_And_Costs[i, 2]); // Cost
                }
            }

            // - OTHER MATH -
            // Total Service and Tax
            total_tax = total_cost * TAX_RATE;

            lblTotalCostServices.Text = total_cost.ToString("C2");
            lblTotalCostTax.Text = total_tax.ToString("C2");

            // Apply Tax to total services
            total_cost += total_tax;

            // Output totals
            lblTotalTime.Text = String.Format("{0,2}:{1,2}", total_time / MINUTES_IN_HOUR, total_time % MINUTES_IN_HOUR);
            lblTotalCost.Text = total_cost.ToString("C2");
        }

        // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        // = = = = = = EVENTS  = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        // = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        // Stylists
        private void lbStylists_SelectedIndexChanged(object sender, EventArgs e)
        {
            stylist_choice = lbStylists.SelectedIndex; //Gets index of selected item in listbox
            UpdateTotals(); //Updates all totals and displays
            lbHaircuts.Visible = true;

            lblChooseHaircut.Visible = true;

            lblChosenStylist.Visible = true;
        }

        // Haircuts
        private void lbHaircuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            haircut_choice = lbHaircuts.SelectedIndex; //Gets index of selected item in listbox
            UpdateTotals(); //Updates all totals and displays

            //Treatment Choice Stuff
            lbTreatments.Visible = true;
            lblChooseTreatment.Visible = true;
            lblChooseTreatmentsSub.Visible = true;
            btnDetailsTreatments.Visible = true; //Details button

            //Itemized List Stuff - Haircuts
            lblChosenHaircut.Visible = true; //Label
            lblCostHaircut.Visible = true; //Value

            //Itemized Total Stuff
            //Labels
            lblItemizedTotalTime.Visible = true;
            lblItemizedServices.Visible = true;
            lblItemizedTax.Visible = true;
            lblItemizedTotal.Visible = true;
            //Numeric Totals
            lblTotalTime.Visible = true;
            lblTotalCost.Visible = true;
            lblItemizedLine.Visible = true; //----------
            lblTotalCostServices.Visible = true;
            lblTotalCostTax.Visible = true;

            //Addon Choice Stuff
            lbAddons.Visible = true;
            lblChooseAddons.Visible = true;
            lblChooseAddonsSub.Visible = true;
            btnDetailsAddons.Visible = true;

            //Confirm btn vsible after sellecting something from every service listbox
            confirmButton.Visible = true;

        }

        // Treatments
        private void lbTreatments_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update Totals
            treatment_choice = lbTreatments.SelectedIndex; //Gets index of selected item in listbox
            UpdateTotals(); //Updates all totals and displays

            //Itemized List Stuff - Treatments
            lblChosenTreatment.Visible = true;
            lblCostTreatments.Visible = true;
        }

        // Add-Ons
        private void lbAddons_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update Totals
            addOns_choice = lbAddons.SelectedIndex; //Gets index of selected item in listbox
            UpdateTotals(); //Updates all totals and displays

            //Itemized List Stuff - Addons
            lblChosenAddons.Visible = true;
            lblCostAddons.Visible = true;

        }

        // Date
        private void lbDateTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            time_choice = cbDates.SelectedIndex;

            pnlBillingInfo.Visible = true;
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            //Date Visibility
            lblTitleDate.Visible = true;
            cbDates.Visible = true;
            lblTitleSubDate.Visible = true;

            //Hide confirm btn
            confirmButton.Visible = false;
        }

        private void btnDetailsTreatments_Click(object sender, EventArgs e)
        {
            string title = "Treatments";
            string message = "";

            message += "**Facials** : a basic rejuvenating skin treatment that includes steam, ";
            message += "exfoliation, extractions, masks, and massage. All facials are tailored and ";
            message += "personalized to address the specific needs of your individual skin type ";
            message += "(dry, oily, or combination skin) and have the maximum benefits.";
            message += "\r\n\r\n**Conditioning Hair Mask** :  a deep hair conditioning treatment used ";
            message += "to heal dry and damaged hair. It contains rich ingredients such as natural oils ";
            message += "and lipids, to nourish and strengthen the hair. \r\n\r\n**Scalp Treatment** : A soothing ";
            message += "and nourishing treatment to help reduce dandruff and prevent dryness, flaking, and irritation.";
            message += " It also helps distribute hair oils evenly, giving off a more healthy and moisturized look.";

            MessageBox.Show(message, title);
        }

        private void btnDetailsAddons_Click(object sender, EventArgs e)
        {
            string title = "Add-ons";
            string message = "";

            message += "**Hot Towel and Facial Hair Trim** : a grooming service where your face is shaved after using a ";
            message += "hot towel to soften your hair and skin. The process of steaming the hair helps open the pores and results ";
            message += "in hair growth. This unique service will include either a trim or a full shave with the razor and using the foam.\r\n\r\n";
            message += "**Simple Hairstyling** : This service includes a quick blowout and the option of straightening, waving, or curling of the hair.\r\n\r\n";
            message += "**Full Foil Color** : A full foil service is the highlighting service of your entire hair.";
            message += "All hair is parted into sections in order to place foils all around, especially underneath your hair; usually this";
            message += "consists of 45 to 100 foils. For those with darker hair looking to completely transform their hair, a full hair foil service will be the best approach.\r\n\r\n";
            message += "**Partial Foil Color** : Partial foils are only focused on certain sections of your hair. ";
            message += "The most common areas that get highlighted are the top half and side sections of your hair, maxing out at around 20 foils. ";
            message += "Partial hair foils are used to create “babylights,” which are intended to subtly brighten overall color.\r\n\r\n";
            message += "**Extension** : Removal** : unsewing the pieces or tracks of weave hair from natural hair.\r\n\r\n";
            message += "**Perm (Curly or straight)** : a thermal chemical ";
            message += "treatment that straightens natural curls and gives a pin-straight, sleek look (if straight perm) or adds texture and curls to ";
            message += "natural hair (if curly perm). The treatment lasts several months, until new hair grows in to replace the hair that was treated.\r\n\r\n";
            message += "**Extension Installation** : individual pieces or tracks of weave hair that are installed onto natural hair to add length and fullness";

            MessageBox.Show(message, title);
        }

        //= = = LOGIN (SKY'S PART) = = =
        private void btnLogin_Click(object sender, EventArgs e)
        {//Checks for valid user within tableUser

            //- VARS -
            string tempName = tbUsername.Text;
            string tempPassword = tbPassword.Text;

            //- CHECK IF USER HAS ACCOUNT -
            for (int i = 1; i < tableUsers.GetLength(0); i++)
            {
                if (tempName == tableUsers[i, 0] && tempPassword == tableUsers[i, 1])
                {//if the name and passwords match the values on record,
                 //They're a valid user
                    indexUserRow = i;
                }
            }

            //- CHECK USER VALIDITY -
            if (!(indexUserRow >= 0 && indexUserRow <= tableUsers.GetLength(0)))
            {// If user not valid

                Console.Beep(); // Console go beep
                lblLoginFailMsg.Visible = true;
                lblLoginFailMsg.Text = "Username or password is incorect!\n";
                lblLoginFailMsg.Text += "Please try again.";
            }
            else
            {// If user valid

                //- ELEMENT VISIBILITY -
                //Make login panel not visible
                pnlLogin.Visible = false;

                //Make service groupbox visible
                gbServices.Visible = true;

                //Stylist Choice Visible
                lbStylists.Visible = true;
                lblChooseStylist.Visible = true;
            }
        }

        // = UPDATE TIME =
        private void cbDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            time_choice = cbDates.SelectedIndex;

            pnlBillingInfo.Visible = true;
        }

        // = = = RECIEPT OUTPUT (JOE'S PART) & CARD VALIDATION (JULIET"S PART) = = =
        private void btnProcess_Click(object sender, EventArgs e)
        {
            //- VARS -
            string fileNameReciept = "CustomerReceipt." + tbName.Text + "." + DateTime.Now.ToString("MM-dd-yyyy HH.mm.ss") + ".txt";
            string fileNameLog = "LogFile.txt";

            string outputRcpt = "";
            string outputLog = "";
            
            //- - - CARD VALIDATION - - -
            cardNumber = tbCardNumber.Text;
            if (!(cardNumber.Length == 15) && !(cardNumber.Length == 16))
            {// if not valid
                lblCardValidity.Text = "Error: Card is invalid.";
                finalCardNumber = "";

                lblRecieptGenerated.Text = "";
            }
            else
            {// if valid
                lblCardValidity.Text = "Card is Valid";

                //- - - BUILD RECIPT OUTPUT - - -
                finalCardNumber = String.Format("{0}{1}", STARS, cardNumber.Substring(cardNumber.Length - 4));

                outputRcpt += string.Format("{0, -10}{1, 55}", "Date: ", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                outputRcpt += string.Format("\n");
                outputRcpt += string.Format("{0, -10}{1, 55}", "Name: ", tbName.Text);
                outputRcpt += string.Format("\n");
                outputRcpt += string.Format("{0, -12}{1, 52}", "Card Number: ", STARS + tbCardNumber.Text.Substring(tbCardNumber.Text.Length - 4));
                outputRcpt += string.Format("\n");
                outputRcpt += string.Format("{0, -10}", cbDates.SelectedItem.ToString());
                outputRcpt += "\n";
                outputRcpt += "-----------------------------------------------------------------";
                outputRcpt += "\n";
                outputRcpt += "\n";
                outputRcpt += string.Format("     {0, -15}", "Stylist:");
                outputRcpt += string.Format("{0}", lblChosenStylist.Text);
                outputRcpt += "\n";
                outputRcpt += string.Format("     {0, -15}", "Haircut Type:");
                outputRcpt += string.Format("{0, -35}", lblChosenHaircut.Text);
                outputRcpt += string.Format("{0, 10}", "$" + lblCostHaircut.Text);
                outputRcpt += "\n";
                outputRcpt += "\n";
                outputRcpt += string.Format("     {0, -15}", "Treatment(s):");
                foreach (int i in lbTreatments.SelectedIndices)
                {
                    outputRcpt += string.Format("{0, -35}{1, 10}\n                    ", treatments_Time_And_Costs[i, 0], "$" + treatments_Time_And_Costs[i, 2]);
                }
                outputRcpt += "\n";
                outputRcpt += string.Format("     {0, -15}", "Addons:");
                foreach (int i in lbAddons.SelectedIndices)
                {
                    outputRcpt += string.Format("{0, -35}{1, 10}\n                    ", addOns_Time_And_Costs[i, 0], "$" + addOns_Time_And_Costs[i, 2]);
                }
                outputRcpt += "\n";
                outputRcpt += "\n";
                outputRcpt += "-----------------------------------------------------------------";
                outputRcpt += "\n";
                outputRcpt += "\n";
                outputRcpt += string.Format("     {0, -15}", "Subtotal:");
                outputRcpt += string.Format("{0, 45}", lblTotalCostServices.Text);
                outputRcpt += "\n";
                outputRcpt += string.Format("     {0, -15}", "Tax:");
                outputRcpt += string.Format("{0, 45}", lblTotalCostTax.Text);
                outputRcpt += "\n";
                outputRcpt += string.Format("     {0, -15}", "Total:");
                outputRcpt += string.Format("{0, 45}", lblTotalCost.Text);
                outputRcpt += string.Format("");

                //- OPEN -
                FileStream fsOut = new FileStream(fileNameReciept, FileMode.Create, FileAccess.Write);
                StreamWriter srOut = new StreamWriter(fsOut);

                //writing receipt to file
                srOut.Write(outputRcpt);

                //- CLOSE -
                srOut.Close();
                fsOut.Close();

                //- - - LOG FILE OUTPUT - - -
                FileStream fileOut;

                if (File.Exists(fileNameLog))
                {
                    fileOut = new FileStream(fileNameLog, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    fileOut = new FileStream(fileNameLog, FileMode.Create, FileAccess.Write);
                }

                StreamWriter streamWriter = new StreamWriter(fileOut);
                outputLog += tbName.Text + ","  + cbDates.SelectedItem.ToString() + "," + tbAdress.Text + "," + lblTotalCost.Text + ",";
                foreach (int i in lbStylists.SelectedIndices)
                {
                    outputLog += string.Format(stylists_Time_And_Costs[i, 0]);
                }

                streamWriter.WriteLine(outputLog);

                streamWriter.Close();
                fileOut.Close();

                //- MSG -
                if (File.Exists(fileNameReciept))
                {
                    lblRecieptGenerated.Text = "Reciept Generated!";
                    btnProcess.Enabled = false;
                }
                else
                {
                    lblRecieptGenerated.Text = "Error generating reciept!";
                }
            }
        }
    }
}