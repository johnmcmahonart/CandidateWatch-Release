import React from 'react';
import List from '@mui/material/List';
import { ICandidateCard } from '../Interfaces/Components';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import  Grid2  from '@mui/material/Unstable_Grid2';

function CandidateCard(props: ICandidateCard) {
    return (
        //<ListItemButton onClick="">
        <Grid2 xs={8} sm="auto">
            <Card sx={{ minWidth: '2%', maxWidth:'100%'}}>

                <CardContent>
                    <Typography sx={{ fontSize: 14 }}>
                        {props.fullName}
                    </Typography>

                    <Typography sx={{ fontSize: 14 }}>
                        {props.district}
                    </Typography>

                    <Typography sx={{ fontSize: 14 }}>
                        {props.party}
                    </Typography>

                    <Typography sx={{ fontSize: 14 }}>
                        {props.electionYear}
                    </Typography>

                    <CardActions>
                        <Button>More Details
                        </Button>
                    </CardActions>
                </CardContent>
            </Card>


        </Grid2>
        

    );
}

export default CandidateCard;